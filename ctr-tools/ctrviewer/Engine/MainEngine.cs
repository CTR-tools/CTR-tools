using CTRFramework.Lang;
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

    public class UserContext
    {
        //user display size
        public Vector2 DisplaySize = new Vector2(1280, 720);

        //selected buffer size
        public Vector2 BufferSize = new Vector2(1280, 720);
    }

    public partial class MainEngine : IDisposable
    {
        public UserContext userContext = new UserContext();

        public Game1 game;

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
        public LineCollection bspBranches = new LineCollection();
        public Dictionary<int, LineCollection> bspLeaves = new Dictionary<int, LineCollection>();


        //sky
        public MGLevel sky;
        public Color BackgroundColor = Color.DarkBlue;

        public MainEngine(Game1 _game)
        {
            game = _game;

            InitializeCameras(game);
            UpdateFOV();
            Subscribe();
        }


        int screenshotstaken = 0;

        public void TakeScreenShot()
        {
            if (screenBuffer is null)
            {
                GameConsole.Write("Screen buffer doesnt exist.");
                return;
            }

            string screenshotFolder = Helpers.PathCombine(Meta.UserPath, "screenshots");
            string screenshotPath = Helpers.PathCombine(screenshotFolder, $"{screenshotstaken.ToString("0000")}_{DateTime.Now.ToString("ddMMyy_hhmmss")}.png");
            Helpers.CheckFolder(screenshotFolder);

            using (var fs = File.Create(screenshotPath))
            {
                screenBuffer.SaveAsPng(fs, screenBuffer.Width, screenBuffer.Height);
            }

            screenshotstaken++;

            GameConsole.Write($"Screenshot saved: {screenshotPath}");
        }

        public void CreateScreenBuffer(int width = 0, int height = 0)
        {
            if (screenBuffer != null)
                screenBuffer.Dispose();

            if (width > 0) userContext.BufferSize.X = width;
            if (height > 0) userContext.BufferSize.Y = height;

            screenBuffer = new RenderTarget2D(
                game.GraphicsDevice,
                (int)userContext.BufferSize.X,
                (int)userContext.BufferSize.Y,
                false, //mipmap
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24Stencil8,
                Settings.AntiAlias ? Settings.AntiAliasLevel : 0,
                RenderTargetUsage.PreserveContents
            );
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

        public void UpdateProjectionMatrices(Viewport viewport)
        {
            Game1.graphics.GraphicsDevice.Viewport = viewport;

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

        public void UpdateAntiAlias()
        {
            Game1.graphics.PreferMultiSampling = Settings.AntiAlias;
            Game1.graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = Settings.AntiAliasLevel;
        }

        public void UpdateAntiAliasAndBuffer()
        {
            UpdateAntiAlias();
            CreateScreenBuffer();
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
            bspBranches = null;
            bspLeaves.Clear();
            ContentVault.Clear();
            instanced.Clear();
            MeshHigh.Clear();
            MeshMed.Clear();
            MeshLow.Clear();
            paths.Clear();
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