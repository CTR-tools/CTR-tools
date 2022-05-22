using CTRFramework.Shared;
using ctrviewer.Engine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace ctrviewer.Engine
{
    public enum CameraType
    {
        DefaultCamera,
        LeftEyeCamera,
        RightEyeCamera,
        SkyCamera
    }

    public partial class MainEngine : IDisposable
    {
        public EngineSettings Settings => EngineSettings.Instance;

        public RenderTarget2D screenBuffer;

        public Dictionary<CameraType, FirstPersonCamera> Cameras = new Dictionary<CameraType, FirstPersonCamera>();

        public List<InstancedModel> external = new List<InstancedModel>();
        public List<InstancedModel> instanced = new List<InstancedModel>();
        public List<InstancedModel> paths = new List<InstancedModel>();

        //hi and low scenes converted to monogame
        public List<MGLevel> MeshHigh = new List<MGLevel>();
        public List<MGLevel> MeshMed = new List<MGLevel>();
        public List<MGLevel> MeshLow = new List<MGLevel>();

        //bounding boxes for visdata
        public List<WireBox> bbox = new List<WireBox>();
        public Dictionary<int, List<WireBox>> bbox2 = new Dictionary<int, List<WireBox>>();


        //sky
        public MGLevel sky;
        public Color BackgroundColor = Color.DarkBlue;

        public MainEngine(Game game)
        {
            InitializeCameras(game);
            UpdateFOV();
            Subscribe();
        }


        int screenshotstaken = 0;

        public void TakeScreenShot()
        {
            if (!Settings.InternalPSXResolution)
            {
                GameConsole.Write("Can't take screenshot without native resolution buffer.");
                return;
            }

            string screenshotFolder = Path.Combine(Meta.UserPath, "screenshots");
            string screenshotPath = Path.Combine(screenshotFolder, $"{screenshotstaken.ToString("0000")}_{DateTime.Now.ToString("ddMMyy_hhmmss")}.png");
            Helpers.CheckFolder(screenshotFolder);

            using (var fs = File.Create(screenshotPath))
            {
                screenBuffer.SaveAsPng(fs, screenBuffer.Width, screenBuffer.Height);
            }

            screenshotstaken++;

            GameConsole.Write($"Screenshot saved: {screenshotPath}");
        }

        public void Subscribe()
        {
            Settings.onFieldOfViewChanged += UpdateFOV;
        }

        public void Unsubscribe()
        {
            Settings.onFieldOfViewChanged -= UpdateFOV;
        }

        public void InitializeCameras(Game game)
        {
            foreach (var camera in (CameraType[])Enum.GetValues(typeof(CameraType)))
                Cameras.Add(camera, new FirstPersonCamera(game));
        }

        public void UpdateProjectionMatrices()
        {
            foreach (var camera in Cameras.Values)
                camera.UpdateProjectionMatrix();
        }

        public void UpdateStereoCamera(CameraType cameraType, float separationValue)
        {
            Vector3 moveVector = Vector3.Transform((cameraType == (Settings.StereoCrossEyed ? CameraType.RightEyeCamera : CameraType.LeftEyeCamera) ? Vector3.Left : Vector3.Right) * separationValue / 100f, Cameras[CameraType.DefaultCamera].GetYawPitchRollMatrix());
            Cameras[cameraType].Position = Cameras[CameraType.DefaultCamera].Position + moveVector;
            Cameras[cameraType].rotationSpeed = Cameras[CameraType.DefaultCamera].rotationSpeed;
            Cameras[cameraType].Target = Cameras[CameraType.DefaultCamera].Target;
        }

        public void UpdateFOV()
        {
            foreach (var camera in Cameras.Values)
                camera.ViewAngle = Settings.FieldOfView;
        }

        public void Draw()
        {
        }

        public void Update(GameTime gameTime)
        {
            foreach (var mg in MeshMed)
                mg.Update(gameTime);

            foreach (var mg in MeshLow)
                mg.Update(gameTime);

            foreach (var im in instanced)
                im.Update(gameTime);

            foreach (var im in paths)
                im.Update(gameTime);
        }

        public void Clear()
        {
            sky = null;
            ContentVault.Clear();
            instanced.Clear();
            MeshHigh.Clear();
            MeshMed.Clear();
            MeshLow.Clear();
            paths.Clear();
            bbox.Clear();
            bbox2.Clear();
        }

        public void Dispose()
        {
            EngineSettings.Save();
            Unsubscribe();
            Cameras.Clear();
            Clear();
        }
    }
}