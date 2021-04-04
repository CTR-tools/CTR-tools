// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild
using Kaitai;
using System.Collections.Generic;
using System.IO;

namespace CTRTools
{

    /// <summary>
    /// kaitai-struct conversion of CTR RAM Mapping Project/ModSDK GameConfig struct
    /// </summary>
    public class CtrGameconfig : KaitaiStruct
    {
        public static CtrGameconfig FromFile(string fileName)
        {
            return new CtrGameconfig(new KaitaiStream(fileName));
        }

        public static CtrGameconfig FromStream(Stream stream)
        {
            return new CtrGameconfig(new KaitaiStream(stream));
        }

        public CtrGameconfig(KaitaiStream p__io, KaitaiStruct p__parent = null, CtrGameconfig p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _gameMode = m_io.ReadU4le();
            _cheats = m_io.ReadU4le();
            _advFlags = m_io.ReadU4le();
            _swapchainIndex = m_io.ReadU4le();
            _ptrSwapchain = new List<uint>((int) (2));
            for (var i = 0; i < 2; i++)
            {
                _ptrSwapchain.Add(m_io.ReadU4le());
            }
            _database = new List<Db>((int) (2));
            for (var i = 0; i < 2; i++)
            {
                _database.Add(new Db(m_io, this, m_root));
            }
            _ptrLev1 = m_io.ReadU4le();
            _prtLev2 = m_io.ReadU4le();
            _cameras = new List<Camera>((int) (4));
            for (var i = 0; i < 4; i++)
            {
                _cameras.Add(new Camera(m_io, this, m_root));
            }
            _skipArray12Entries = new List<byte[]>((int) (12));
            for (var i = 0; i < 12; i++)
            {
                _skipArray12Entries.Add(m_io.ReadBytes(296));
            }
            _cameraUi = new Camera(m_io, this, m_root);
            _driverCameras = new List<CameraDc>((int) (4));
            for (var i = 0; i < 4; i++)
            {
                _driverCameras.Add(new CameraDc(m_io, this, m_root));
            }
            _data0xc0 = m_io.ReadBytes(192);
            _ptrOt = new List<uint>((int) (2));
            for (var i = 0; i < 2; i++)
            {
                _ptrOt.Add(m_io.ReadU4le());
            }
            _pools = new PoolStruct(m_io, this, m_root);
            _levelId = m_io.ReadU4le();
            _levelName = System.Text.Encoding.GetEncoding("ascii").GetString(KaitaiStream.BytesTerminate(m_io.ReadBytes(36), 0, false));
            _skipData20 = m_io.ReadBytes(32);
            _data0xc02 = m_io.ReadBytes(192);
            _ptrClod = m_io.ReadU4le();
            _ptrDustpuff = m_io.ReadU4le();
            _ptrSmoking = m_io.ReadU4le();
            _ptrSparkle = m_io.ReadU4le();
            _ptrIconUnk = m_io.ReadU4le();
            _threadBucketsArray = new ThreadBuckets(m_io, this, m_root);
            _ptrRenderBucketInstance = m_io.ReadU4le();
            _skip3 = m_io.ReadBytes(16);
            _numPlayers = m_io.ReadU1();
            _numControllers = m_io.ReadU1();
            _unkUnused = m_io.ReadU1();
            _numBots = m_io.ReadU1();
            _dataBetweenScreensAndTimer = m_io.ReadBytes(48);
            _unkFlipPositiveNegative = m_io.ReadU4le();
            _unk012 = m_io.ReadU4le();
            _frameTimerNoExceptions = m_io.ReadU4le();
            _frameTimerNotPaused = m_io.ReadU4le();
            _timer = m_io.ReadU4le();
            _variousTimers = new List<uint>((int) (7));
            for (var i = 0; i < 7; i++)
            {
                _variousTimers.Add(m_io.ReadU4le());
            }
            _trafficLightsTimer = m_io.ReadS2le();
            _unkShortTrafficMightBeInt = m_io.ReadS2le();
            _elapsedEventTime = m_io.ReadU4le();
            _data1cAlways0 = m_io.ReadBytes(28);
            _unk1d30 = m_io.ReadU1();
            _hudFlags = m_io.ReadU1();
            _boolDemoMode = m_io.ReadU1();
            _numLaps = m_io.ReadU1();
            _unkTimerCooldownSimilarTo1d36 = m_io.ReadU2le();
            _timerEndOfRaceVs = m_io.ReadU2le();
            _cooldownFromPauseUntilUnpause = m_io.ReadU1();
            _cooldownFromUnpauseUntilPause = m_io.ReadU1();
            _advPausePage = m_io.ReadU2le();
            _unkRelatedToRelicRace = m_io.ReadU4le();
            _lapIndexNewBest = m_io.ReadU4le();
            _unknownFlags1d44 = m_io.ReadU4le();
            _unknown1d48NotFound = m_io.ReadU1();
            _last68DifferenceSep3UsaRetail = m_io.ReadU1();
            _notFoundInCode1 = m_io.ReadU2le();
            _test = m_io.ReadU2le();
            _notFoundInCode2 = m_io.ReadU2le();
            _notFoundInCode3 = m_io.ReadU2le();
            _skip0x24 = m_io.ReadBytes(36);
            _unknownNotinsep3Again = m_io.ReadU2le();
            _timeToBeatInTimeTrialForCurrentEvent = m_io.ReadU4le();
            _skip5 = m_io.ReadBytes(8);
            _originalEventTime = m_io.ReadU4le();
            _battleSetupStruct = new BattleSetup(m_io, this, m_root);
            _frozenTimeRemaining = m_io.ReadU4le();
            _timeCrateTypeSmashed = m_io.ReadU4le();
            _numCrystalsInLev = m_io.ReadU4le();
            _timeCratesInLev = m_io.ReadU4le();
            _numTrophies = m_io.ReadU4le();
            _numRelics = m_io.ReadU4le();
            _numKeys = m_io.ReadU4le();
            _total = m_io.ReadU4le();
            _red = m_io.ReadU4le();
            _green = m_io.ReadU4le();
            _blue = m_io.ReadU4le();
            _yellow = m_io.ReadU4le();
            _purlpe = m_io.ReadU4le();
            _completionPercent = m_io.ReadU4le();
            _cupId = m_io.ReadU4le();
            _trackIndex = m_io.ReadU4le();
            _points = new List<uint>((int) (8));
            for (var i = 0; i < 8; i++)
            {
                _points.Add(m_io.ReadU4le());
            }
            _standingPoints = m_io.ReadBytes(48);
            _currLev = m_io.ReadU4le();
            _prevLev = m_io.ReadU4le();
            _bossId = m_io.ReadU4le();
            _arcadeDifficulty = m_io.ReadU4le();
            _numMissiles = m_io.ReadU4le();
            _numPlayersWith3Misiles = m_io.ReadU4le();
            _rainVar = m_io.ReadU4le();
            _ptrRedOff = m_io.ReadU4le();
            _ptrRedOn = m_io.ReadU4le();
            _ptrGreenOff = m_io.ReadU4le();
            _ptrGreenOn = m_io.ReadU4le();
            _demoCountdownTimer = m_io.ReadU4le();
            _unk1Afterdemo = m_io.ReadU4le();
            _unk2Afterdemo = m_io.ReadU4le();
            _unk3Afterdemo = m_io.ReadU4le();
            _ptrIcons = new List<uint>((int) (136));
            for (var i = 0; i < 136; i++)
            {
                _ptrIcons.Add(m_io.ReadU4le());
            }
            _unk210c = m_io.ReadU4le();
            _unk2110 = m_io.ReadU4le();
            _iconGroup = new List<uint>((int) (17));
            for (var i = 0; i < 17; i++)
            {
                _iconGroup.Add(m_io.ReadU4le());
            }
            _unk2158 = m_io.ReadU4le();
            _unk215c = m_io.ReadU4le();
            _modelPtr = new List<uint>((int) (227));
            for (var i = 0; i < 227; i++)
            {
                _modelPtr.Add(m_io.ReadU4le());
            }
            _ptrDrivers = new List<uint>((int) (8));
            for (var i = 0; i < 8; i++)
            {
                _ptrDrivers.Add(m_io.ReadU4le());
            }
            _ptrDriversOrdered = new List<uint>((int) (8));
            for (var i = 0; i < 8; i++)
            {
                _ptrDriversOrdered.Add(m_io.ReadU4le());
            }
            _deadcoedStruct = new Deadcoed(m_io, this, m_root);
            _fillerNull1 = m_io.ReadBytes(12);
            _overlayLoaded1 = m_io.ReadU1();
            _overlayLoaded2 = m_io.ReadU1();
            _overlayLoaded3Neverused = m_io.ReadU1();
            _overlayLoaded4 = m_io.ReadU1();
            _skipFinalFiller = m_io.ReadBytes(36);
            _frameDelayMaybe = m_io.ReadU4le();
            _renderflags = m_io.ReadU4le();
            _clockEffectEnabled = m_io.ReadU2le();
            _valueAfterClockEffect = m_io.ReadU2le();
            _someHowlBankData = new List<byte>((int) (4));
            for (var i = 0; i < 4; i++)
            {
                _someHowlBankData.Add(m_io.ReadU1());
            }
            _someValueBeforePositions1 = m_io.ReadU1();
            _someValueBeforePositions2 = m_io.ReadU1();
            _currentHumanPlayerPosition = new List<byte>((int) (8));
            for (var i = 0; i < 8; i++)
            {
                _currentHumanPlayerPosition.Add(m_io.ReadU1());
            }
            _finalShort = m_io.ReadS2le();
        }
        public partial class CameraDc : KaitaiStruct
        {
            public static CameraDc FromFile(string fileName)
            {
                return new CameraDc(new KaitaiStream(fileName));
            }

            public CameraDc(KaitaiStream p__io, CtrGameconfig p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _driverIndex = m_io.ReadU4le();
                _data0x44 = m_io.ReadBytes(64);
                _ptrDriver = m_io.ReadU4le();
                _ptrCamera = m_io.ReadU4le();
                _data0x90 = m_io.ReadBytes(144);
            }
            private uint _driverIndex;
            private byte[] _data0x44;
            private uint _ptrDriver;
            private uint _ptrCamera;
            private byte[] _data0x90;
            private CtrGameconfig m_root;
            private CtrGameconfig m_parent;
            public uint DriverIndex { get { return _driverIndex; } }
            public byte[] Data0x44 { get { return _data0x44; } }
            public uint PtrDriver { get { return _ptrDriver; } }
            public uint PtrCamera { get { return _ptrCamera; } }
            public byte[] Data0x90 { get { return _data0x90; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig M_Parent { get { return m_parent; } }
        }
        public partial class Vector3s : KaitaiStruct
        {
            public static Vector3s FromFile(string fileName)
            {
                return new Vector3s(new KaitaiStream(fileName));
            }

            public Vector3s(KaitaiStream p__io, CtrGameconfig.Camera p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _x = m_io.ReadS2le();
                _y = m_io.ReadS2le();
                _z = m_io.ReadS2le();
            }
            private short _x;
            private short _y;
            private short _z;
            private CtrGameconfig m_root;
            private CtrGameconfig.Camera m_parent;
            public short X { get { return _x; } }
            public short Y { get { return _y; } }
            public short Z { get { return _z; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig.Camera M_Parent { get { return m_parent; } }
        }
        public partial class ThreadBuckets : KaitaiStruct
        {
            public static ThreadBuckets FromFile(string fileName)
            {
                return new ThreadBuckets(new KaitaiStream(fileName));
            }

            public ThreadBuckets(KaitaiStream p__io, CtrGameconfig p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _player = new ThreadBucket(m_io, this, m_root);
                _robot = new ThreadBucket(m_io, this, m_root);
                _ghost = new ThreadBucket(m_io, this, m_root);
                _static = new ThreadBucket(m_io, this, m_root);
                _mine = new ThreadBucket(m_io, this, m_root);
                _warppad = new ThreadBucket(m_io, this, m_root);
                _tracking = new ThreadBucket(m_io, this, m_root);
                _burst = new ThreadBucket(m_io, this, m_root);
                _blowup = new ThreadBucket(m_io, this, m_root);
                _turbo = new ThreadBucket(m_io, this, m_root);
                _spider = new ThreadBucket(m_io, this, m_root);
                _follower = new ThreadBucket(m_io, this, m_root);
                _startText = new ThreadBucket(m_io, this, m_root);
                _other = new ThreadBucket(m_io, this, m_root);
                _akuAku = new ThreadBucket(m_io, this, m_root);
                _camera = new ThreadBucket(m_io, this, m_root);
                _hud = new ThreadBucket(m_io, this, m_root);
                _pause = new ThreadBucket(m_io, this, m_root);
            }
            private ThreadBucket _player;
            private ThreadBucket _robot;
            private ThreadBucket _ghost;
            private ThreadBucket _static;
            private ThreadBucket _mine;
            private ThreadBucket _warppad;
            private ThreadBucket _tracking;
            private ThreadBucket _burst;
            private ThreadBucket _blowup;
            private ThreadBucket _turbo;
            private ThreadBucket _spider;
            private ThreadBucket _follower;
            private ThreadBucket _startText;
            private ThreadBucket _other;
            private ThreadBucket _akuAku;
            private ThreadBucket _camera;
            private ThreadBucket _hud;
            private ThreadBucket _pause;
            private CtrGameconfig m_root;
            private CtrGameconfig m_parent;
            public ThreadBucket Player { get { return _player; } }
            public ThreadBucket Robot { get { return _robot; } }
            public ThreadBucket Ghost { get { return _ghost; } }
            public ThreadBucket Static { get { return _static; } }
            public ThreadBucket Mine { get { return _mine; } }
            public ThreadBucket Warppad { get { return _warppad; } }
            public ThreadBucket Tracking { get { return _tracking; } }
            public ThreadBucket Burst { get { return _burst; } }
            public ThreadBucket Blowup { get { return _blowup; } }
            public ThreadBucket Turbo { get { return _turbo; } }
            public ThreadBucket Spider { get { return _spider; } }
            public ThreadBucket Follower { get { return _follower; } }
            public ThreadBucket StartText { get { return _startText; } }
            public ThreadBucket Other { get { return _other; } }
            public ThreadBucket AkuAku { get { return _akuAku; } }
            public ThreadBucket Camera { get { return _camera; } }
            public ThreadBucket Hud { get { return _hud; } }
            public ThreadBucket Pause { get { return _pause; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig M_Parent { get { return m_parent; } }
        }
        public partial class ThreadBucket : KaitaiStruct
        {
            public static ThreadBucket FromFile(string fileName)
            {
                return new ThreadBucket(new KaitaiStream(fileName));
            }

            public ThreadBucket(KaitaiStream p__io, CtrGameconfig.ThreadBuckets p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _ptrThread = m_io.ReadU4le();
                _ptrName = m_io.ReadU4le();
                _ptrShortName = m_io.ReadU4le();
                _unk1 = m_io.ReadU4le();
                _unk2 = m_io.ReadU4le();
            }
            private uint _ptrThread;
            private uint _ptrName;
            private uint _ptrShortName;
            private uint _unk1;
            private uint _unk2;
            private CtrGameconfig m_root;
            private CtrGameconfig.ThreadBuckets m_parent;
            public uint PtrThread { get { return _ptrThread; } }
            public uint PtrName { get { return _ptrName; } }
            public uint PtrShortName { get { return _ptrShortName; } }
            public uint Unk1 { get { return _unk1; } }
            public uint Unk2 { get { return _unk2; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig.ThreadBuckets M_Parent { get { return m_parent; } }
        }
        public partial class Otmem : KaitaiStruct
        {
            public static Otmem FromFile(string fileName)
            {
                return new Otmem(new KaitaiStream(fileName));
            }

            public Otmem(KaitaiStream p__io, CtrGameconfig.Db p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _size = m_io.ReadU4le();
                _ptrStart = m_io.ReadU4le();
                _ptrEnd = m_io.ReadU4le();
                _ptrCurr = m_io.ReadU4le();
                _ptrStartPlusFour = m_io.ReadU4le();
            }
            private uint _size;
            private uint _ptrStart;
            private uint _ptrEnd;
            private uint _ptrCurr;
            private uint _ptrStartPlusFour;
            private CtrGameconfig m_root;
            private CtrGameconfig.Db m_parent;
            public uint Size { get { return _size; } }
            public uint PtrStart { get { return _ptrStart; } }
            public uint PtrEnd { get { return _ptrEnd; } }
            public uint PtrCurr { get { return _ptrCurr; } }
            public uint PtrStartPlusFour { get { return _ptrStartPlusFour; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig.Db M_Parent { get { return m_parent; } }
        }
        public partial class Camera : KaitaiStruct
        {
            public static Camera FromFile(string fileName)
            {
                return new Camera(new KaitaiStream(fileName));
            }

            public Camera(KaitaiStream p__io, CtrGameconfig p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _position = new Vector3s(m_io, this, m_root);
                _rotation = new Vector3s(m_io, this, m_root);
                _unk1 = new Vector3s(m_io, this, m_root);
                _fadeInCurrent = m_io.ReadS2le();
                _fadeinTarget = m_io.ReadS2le();
                _fadeinStep = m_io.ReadS2le();
                _unk3 = m_io.ReadU4le();
                _vpLocation = new Vector2s(m_io, this, m_root);
                _vpSize = new Vector2s(m_io, this, m_root);
                _skip = m_io.ReadBytes(208);
                _ptrOt = m_io.ReadU4le();
                _skip2 = m_io.ReadBytes(24);
            }
            private Vector3s _position;
            private Vector3s _rotation;
            private Vector3s _unk1;
            private short _fadeInCurrent;
            private short _fadeinTarget;
            private short _fadeinStep;
            private uint _unk3;
            private Vector2s _vpLocation;
            private Vector2s _vpSize;
            private byte[] _skip;
            private uint _ptrOt;
            private byte[] _skip2;
            private CtrGameconfig m_root;
            private CtrGameconfig m_parent;
            public Vector3s Position { get { return _position; } }
            public Vector3s Rotation { get { return _rotation; } }
            public Vector3s Unk1 { get { return _unk1; } }
            public short FadeInCurrent { get { return _fadeInCurrent; } }
            public short FadeinTarget { get { return _fadeinTarget; } }
            public short FadeinStep { get { return _fadeinStep; } }
            public uint Unk3 { get { return _unk3; } }
            public Vector2s VpLocation { get { return _vpLocation; } }
            public Vector2s VpSize { get { return _vpSize; } }
            public byte[] Skip { get { return _skip; } }
            public uint PtrOt { get { return _ptrOt; } }
            public byte[] Skip2 { get { return _skip2; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig M_Parent { get { return m_parent; } }
        }
        public partial class Db : KaitaiStruct
        {
            public static Db FromFile(string fileName)
            {
                return new Db(new KaitaiStream(fileName));
            }

            public Db(KaitaiStream p__io, CtrGameconfig p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _drawEnv = m_io.ReadBytes(92);
                _dispEnv = m_io.ReadBytes(20);
                _unkPrimMemRelated = m_io.ReadU4le();
                _primMem = new Primmem(m_io, this, m_root);
                _otMem = new Otmem(m_io, this, m_root);
            }
            private byte[] _drawEnv;
            private byte[] _dispEnv;
            private uint _unkPrimMemRelated;
            private Primmem _primMem;
            private Otmem _otMem;
            private CtrGameconfig m_root;
            private CtrGameconfig m_parent;
            public byte[] DrawEnv { get { return _drawEnv; } }
            public byte[] DispEnv { get { return _dispEnv; } }
            public uint UnkPrimMemRelated { get { return _unkPrimMemRelated; } }
            public Primmem PrimMem { get { return _primMem; } }
            public Otmem OtMem { get { return _otMem; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig M_Parent { get { return m_parent; } }
        }
        public partial class BattleSetup : KaitaiStruct
        {
            public static BattleSetup FromFile(string fileName)
            {
                return new BattleSetup(new KaitaiStream(fileName));
            }

            public BattleSetup(KaitaiStream p__io, CtrGameconfig p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _lifeLimit = m_io.ReadU4le();
                _killLimit = m_io.ReadU4le();
                _unkRelatedToTeamSquares = new List<uint>((int) (4));
                for (var i = 0; i < 4; i++)
                {
                    _unkRelatedToTeamSquares.Add(m_io.ReadU4le());
                }
                _weaponsEnabled = m_io.ReadU4le();
                _playerTeams = new List<byte>((int) (4));
                for (var i = 0; i < 4; i++)
                {
                    _playerTeams.Add(m_io.ReadU1());
                }
                _skip = m_io.ReadBytes(48);
                _teamFlags = m_io.ReadU4le();
                _numTeams = m_io.ReadU4le();
                _unkWeapons = m_io.ReadBytes(64);
            }
            private uint _lifeLimit;
            private uint _killLimit;
            private List<uint> _unkRelatedToTeamSquares;
            private uint _weaponsEnabled;
            private List<byte> _playerTeams;
            private byte[] _skip;
            private uint _teamFlags;
            private uint _numTeams;
            private byte[] _unkWeapons;
            private CtrGameconfig m_root;
            private CtrGameconfig m_parent;
            public uint LifeLimit { get { return _lifeLimit; } }
            public uint KillLimit { get { return _killLimit; } }
            public List<uint> UnkRelatedToTeamSquares { get { return _unkRelatedToTeamSquares; } }
            public uint WeaponsEnabled { get { return _weaponsEnabled; } }
            public List<byte> PlayerTeams { get { return _playerTeams; } }
            public byte[] Skip { get { return _skip; } }
            public uint TeamFlags { get { return _teamFlags; } }
            public uint NumTeams { get { return _numTeams; } }
            public byte[] UnkWeapons { get { return _unkWeapons; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig M_Parent { get { return m_parent; } }
        }
        public partial class PoolStruct : KaitaiStruct
        {
            public static PoolStruct FromFile(string fileName)
            {
                return new PoolStruct(new KaitaiStream(fileName));
            }

            public PoolStruct(KaitaiStream p__io, CtrGameconfig p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _thread = new AllocPool(m_io, this, m_root);
                _instance = new AllocPool(m_io, this, m_root);
                _smallStack = new AllocPool(m_io, this, m_root);
                _mediumStack = new AllocPool(m_io, this, m_root);
                _largeStack = new AllocPool(m_io, this, m_root);
                _particle = new AllocPool(m_io, this, m_root);
                _oscillator = new AllocPool(m_io, this, m_root);
                _rain = new AllocPool(m_io, this, m_root);
            }
            private AllocPool _thread;
            private AllocPool _instance;
            private AllocPool _smallStack;
            private AllocPool _mediumStack;
            private AllocPool _largeStack;
            private AllocPool _particle;
            private AllocPool _oscillator;
            private AllocPool _rain;
            private CtrGameconfig m_root;
            private CtrGameconfig m_parent;
            public AllocPool Thread { get { return _thread; } }
            public AllocPool Instance { get { return _instance; } }
            public AllocPool SmallStack { get { return _smallStack; } }
            public AllocPool MediumStack { get { return _mediumStack; } }
            public AllocPool LargeStack { get { return _largeStack; } }
            public AllocPool Particle { get { return _particle; } }
            public AllocPool Oscillator { get { return _oscillator; } }
            public AllocPool Rain { get { return _rain; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig M_Parent { get { return m_parent; } }
        }
        public partial class AllocPool : KaitaiStruct
        {
            public static AllocPool FromFile(string fileName)
            {
                return new AllocPool(new KaitaiStream(fileName));
            }

            public AllocPool(KaitaiStream p__io, CtrGameconfig.PoolStruct p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _ptrLast = m_io.ReadU4le();
                _ptrFirst = m_io.ReadU4le();
                _numEntries = m_io.ReadU4le();
                _ptrChildPool = m_io.ReadU4le();
                _unk1 = m_io.ReadU4le();
                _unk2 = m_io.ReadU4le();
                _numEntriesMax = m_io.ReadU4le();
                _entrySize = m_io.ReadU4le();
                _allocSize = m_io.ReadU4le();
                _ptrAlloc = m_io.ReadU4le();
            }
            private uint _ptrLast;
            private uint _ptrFirst;
            private uint _numEntries;
            private uint _ptrChildPool;
            private uint _unk1;
            private uint _unk2;
            private uint _numEntriesMax;
            private uint _entrySize;
            private uint _allocSize;
            private uint _ptrAlloc;
            private CtrGameconfig m_root;
            private CtrGameconfig.PoolStruct m_parent;
            public uint PtrLast { get { return _ptrLast; } }
            public uint PtrFirst { get { return _ptrFirst; } }
            public uint NumEntries { get { return _numEntries; } }
            public uint PtrChildPool { get { return _ptrChildPool; } }
            public uint Unk1 { get { return _unk1; } }
            public uint Unk2 { get { return _unk2; } }
            public uint NumEntriesMax { get { return _numEntriesMax; } }
            public uint EntrySize { get { return _entrySize; } }
            public uint AllocSize { get { return _allocSize; } }
            public uint PtrAlloc { get { return _ptrAlloc; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig.PoolStruct M_Parent { get { return m_parent; } }
        }
        public partial class Deadcoed : KaitaiStruct
        {
            public static Deadcoed FromFile(string fileName)
            {
                return new Deadcoed(new KaitaiStream(fileName));
            }

            public Deadcoed(KaitaiStream p__io, CtrGameconfig p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _unk1 = m_io.ReadU4le();
                _unk2 = m_io.ReadU4le();
            }
            private uint _unk1;
            private uint _unk2;
            private CtrGameconfig m_root;
            private CtrGameconfig m_parent;
            public uint Unk1 { get { return _unk1; } }
            public uint Unk2 { get { return _unk2; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig M_Parent { get { return m_parent; } }
        }
        public partial class Primmem : KaitaiStruct
        {
            public static Primmem FromFile(string fileName)
            {
                return new Primmem(new KaitaiStream(fileName));
            }

            public Primmem(KaitaiStream p__io, CtrGameconfig.Db p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _size = m_io.ReadU4le();
                _ptrStart = m_io.ReadU4le();
                _ptrEnd = m_io.ReadU4le();
                _ptrCurr = m_io.ReadU4le();
                _ptrEndMin100 = m_io.ReadU4le();
                _unk1 = m_io.ReadU4le();
                _ptrUnk2 = m_io.ReadU4le();
            }
            private uint _size;
            private uint _ptrStart;
            private uint _ptrEnd;
            private uint _ptrCurr;
            private uint _ptrEndMin100;
            private uint _unk1;
            private uint _ptrUnk2;
            private CtrGameconfig m_root;
            private CtrGameconfig.Db m_parent;
            public uint Size { get { return _size; } }
            public uint PtrStart { get { return _ptrStart; } }
            public uint PtrEnd { get { return _ptrEnd; } }
            public uint PtrCurr { get { return _ptrCurr; } }
            public uint PtrEndMin100 { get { return _ptrEndMin100; } }
            public uint Unk1 { get { return _unk1; } }
            public uint PtrUnk2 { get { return _ptrUnk2; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig.Db M_Parent { get { return m_parent; } }
        }
        public partial class Vector2s : KaitaiStruct
        {
            public static Vector2s FromFile(string fileName)
            {
                return new Vector2s(new KaitaiStream(fileName));
            }

            public Vector2s(KaitaiStream p__io, CtrGameconfig.Camera p__parent = null, CtrGameconfig p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _x = m_io.ReadS2le();
                _y = m_io.ReadS2le();
            }
            private short _x;
            private short _y;
            private CtrGameconfig m_root;
            private CtrGameconfig.Camera m_parent;
            public short X { get { return _x; } }
            public short Y { get { return _y; } }
            public CtrGameconfig M_Root { get { return m_root; } }
            public CtrGameconfig.Camera M_Parent { get { return m_parent; } }
        }
        private uint _gameMode;
        private uint _cheats;
        private uint _advFlags;
        private uint _swapchainIndex;
        private List<uint> _ptrSwapchain;
        private List<Db> _database;
        private uint _ptrLev1;
        private uint _prtLev2;
        private List<Camera> _cameras;
        private List<byte[]> _skipArray12Entries;
        private Camera _cameraUi;
        private List<CameraDc> _driverCameras;
        private byte[] _data0xc0;
        private List<uint> _ptrOt;
        private PoolStruct _pools;
        private uint _levelId;
        private string _levelName;
        private byte[] _skipData20;
        private byte[] _data0xc02;
        private uint _ptrClod;
        private uint _ptrDustpuff;
        private uint _ptrSmoking;
        private uint _ptrSparkle;
        private uint _ptrIconUnk;
        private ThreadBuckets _threadBucketsArray;
        private uint _ptrRenderBucketInstance;
        private byte[] _skip3;
        private byte _numPlayers;
        private byte _numControllers;
        private byte _unkUnused;
        private byte _numBots;
        private byte[] _dataBetweenScreensAndTimer;
        private uint _unkFlipPositiveNegative;
        private uint _unk012;
        private uint _frameTimerNoExceptions;
        private uint _frameTimerNotPaused;
        private uint _timer;
        private List<uint> _variousTimers;
        private short _trafficLightsTimer;
        private short _unkShortTrafficMightBeInt;
        private uint _elapsedEventTime;
        private byte[] _data1cAlways0;
        private byte _unk1d30;
        private byte _hudFlags;
        private byte _boolDemoMode;
        private byte _numLaps;
        private ushort _unkTimerCooldownSimilarTo1d36;
        private ushort _timerEndOfRaceVs;
        private byte _cooldownFromPauseUntilUnpause;
        private byte _cooldownFromUnpauseUntilPause;
        private ushort _advPausePage;
        private uint _unkRelatedToRelicRace;
        private uint _lapIndexNewBest;
        private uint _unknownFlags1d44;
        private byte _unknown1d48NotFound;
        private byte _last68DifferenceSep3UsaRetail;
        private ushort _notFoundInCode1;
        private ushort _test;
        private ushort _notFoundInCode2;
        private ushort _notFoundInCode3;
        private byte[] _skip0x24;
        private ushort _unknownNotinsep3Again;
        private uint _timeToBeatInTimeTrialForCurrentEvent;
        private byte[] _skip5;
        private uint _originalEventTime;
        private BattleSetup _battleSetupStruct;
        private uint _frozenTimeRemaining;
        private uint _timeCrateTypeSmashed;
        private uint _numCrystalsInLev;
        private uint _timeCratesInLev;
        private uint _numTrophies;
        private uint _numRelics;
        private uint _numKeys;
        private uint _total;
        private uint _red;
        private uint _green;
        private uint _blue;
        private uint _yellow;
        private uint _purlpe;
        private uint _completionPercent;
        private uint _cupId;
        private uint _trackIndex;
        private List<uint> _points;
        private byte[] _standingPoints;
        private uint _currLev;
        private uint _prevLev;
        private uint _bossId;
        private uint _arcadeDifficulty;
        private uint _numMissiles;
        private uint _numPlayersWith3Misiles;
        private uint _rainVar;
        private uint _ptrRedOff;
        private uint _ptrRedOn;
        private uint _ptrGreenOff;
        private uint _ptrGreenOn;
        private uint _demoCountdownTimer;
        private uint _unk1Afterdemo;
        private uint _unk2Afterdemo;
        private uint _unk3Afterdemo;
        private List<uint> _ptrIcons;
        private uint _unk210c;
        private uint _unk2110;
        private List<uint> _iconGroup;
        private uint _unk2158;
        private uint _unk215c;
        private List<uint> _modelPtr;
        private List<uint> _ptrDrivers;
        private List<uint> _ptrDriversOrdered;
        private Deadcoed _deadcoedStruct;
        private byte[] _fillerNull1;
        private byte _overlayLoaded1;
        private byte _overlayLoaded2;
        private byte _overlayLoaded3Neverused;
        private byte _overlayLoaded4;
        private byte[] _skipFinalFiller;
        private uint _frameDelayMaybe;
        private uint _renderflags;
        private ushort _clockEffectEnabled;
        private ushort _valueAfterClockEffect;
        private List<byte> _someHowlBankData;
        private byte _someValueBeforePositions1;
        private byte _someValueBeforePositions2;
        private List<byte> _currentHumanPlayerPosition;
        private short _finalShort;
        private CtrGameconfig m_root;
        private KaitaiStruct m_parent;
        public uint GameMode { get { return _gameMode; } }
        public uint Cheats { get { return _cheats; } }
        public uint AdvFlags { get { return _advFlags; } }
        public uint SwapchainIndex { get { return _swapchainIndex; } }
        public List<uint> PtrSwapchain { get { return _ptrSwapchain; } }
        public List<Db> Database { get { return _database; } }
        public uint PtrLev1 { get { return _ptrLev1; } }
        public uint PrtLev2 { get { return _prtLev2; } }
        public List<Camera> Cameras { get { return _cameras; } }
        public List<byte[]> SkipArray12Entries { get { return _skipArray12Entries; } }
        public Camera CameraUi { get { return _cameraUi; } }
        public List<CameraDc> DriverCameras { get { return _driverCameras; } }
        public byte[] Data0xc0 { get { return _data0xc0; } }
        public List<uint> PtrOt { get { return _ptrOt; } }
        public PoolStruct Pools { get { return _pools; } }
        public uint LevelId { get { return _levelId; } }
        public string LevelName { get { return _levelName; } }
        public byte[] SkipData20 { get { return _skipData20; } }
        public byte[] Data0xc02 { get { return _data0xc02; } }
        public uint PtrClod { get { return _ptrClod; } }
        public uint PtrDustpuff { get { return _ptrDustpuff; } }
        public uint PtrSmoking { get { return _ptrSmoking; } }
        public uint PtrSparkle { get { return _ptrSparkle; } }
        public uint PtrIconUnk { get { return _ptrIconUnk; } }
        public ThreadBuckets ThreadBucketsArray { get { return _threadBucketsArray; } }
        public uint PtrRenderBucketInstance { get { return _ptrRenderBucketInstance; } }
        public byte[] Skip3 { get { return _skip3; } }
        public byte NumPlayers { get { return _numPlayers; } }
        public byte NumControllers { get { return _numControllers; } }
        public byte UnkUnused { get { return _unkUnused; } }
        public byte NumBots { get { return _numBots; } }
        public byte[] DataBetweenScreensAndTimer { get { return _dataBetweenScreensAndTimer; } }
        public uint UnkFlipPositiveNegative { get { return _unkFlipPositiveNegative; } }
        public uint Unk012 { get { return _unk012; } }
        public uint FrameTimerNoExceptions { get { return _frameTimerNoExceptions; } }
        public uint FrameTimerNotPaused { get { return _frameTimerNotPaused; } }
        public uint Timer { get { return _timer; } }
        public List<uint> VariousTimers { get { return _variousTimers; } }
        public short TrafficLightsTimer { get { return _trafficLightsTimer; } }
        public short UnkShortTrafficMightBeInt { get { return _unkShortTrafficMightBeInt; } }
        public uint ElapsedEventTime { get { return _elapsedEventTime; } }
        public byte[] Data1cAlways0 { get { return _data1cAlways0; } }
        public byte Unk1d30 { get { return _unk1d30; } }
        public byte HudFlags { get { return _hudFlags; } }
        public byte BoolDemoMode { get { return _boolDemoMode; } }
        public byte NumLaps { get { return _numLaps; } }
        public ushort UnkTimerCooldownSimilarTo1d36 { get { return _unkTimerCooldownSimilarTo1d36; } }
        public ushort TimerEndOfRaceVs { get { return _timerEndOfRaceVs; } }
        public byte CooldownFromPauseUntilUnpause { get { return _cooldownFromPauseUntilUnpause; } }
        public byte CooldownFromUnpauseUntilPause { get { return _cooldownFromUnpauseUntilPause; } }
        public ushort AdvPausePage { get { return _advPausePage; } }
        public uint UnkRelatedToRelicRace { get { return _unkRelatedToRelicRace; } }
        public uint LapIndexNewBest { get { return _lapIndexNewBest; } }
        public uint UnknownFlags1d44 { get { return _unknownFlags1d44; } }
        public byte Unknown1d48NotFound { get { return _unknown1d48NotFound; } }
        public byte Last68DifferenceSep3UsaRetail { get { return _last68DifferenceSep3UsaRetail; } }
        public ushort NotFoundInCode1 { get { return _notFoundInCode1; } }
        public ushort Test { get { return _test; } }
        public ushort NotFoundInCode2 { get { return _notFoundInCode2; } }
        public ushort NotFoundInCode3 { get { return _notFoundInCode3; } }
        public byte[] Skip0x24 { get { return _skip0x24; } }
        public ushort UnknownNotinsep3Again { get { return _unknownNotinsep3Again; } }
        public uint TimeToBeatInTimeTrialForCurrentEvent { get { return _timeToBeatInTimeTrialForCurrentEvent; } }
        public byte[] Skip5 { get { return _skip5; } }
        public uint OriginalEventTime { get { return _originalEventTime; } }
        public BattleSetup BattleSetupStruct { get { return _battleSetupStruct; } }
        public uint FrozenTimeRemaining { get { return _frozenTimeRemaining; } }
        public uint TimeCrateTypeSmashed { get { return _timeCrateTypeSmashed; } }
        public uint NumCrystalsInLev { get { return _numCrystalsInLev; } }
        public uint TimeCratesInLev { get { return _timeCratesInLev; } }
        public uint NumTrophies { get { return _numTrophies; } }
        public uint NumRelics { get { return _numRelics; } }
        public uint NumKeys { get { return _numKeys; } }
        public uint Total { get { return _total; } }
        public uint Red { get { return _red; } }
        public uint Green { get { return _green; } }
        public uint Blue { get { return _blue; } }
        public uint Yellow { get { return _yellow; } }
        public uint Purlpe { get { return _purlpe; } }
        public uint CompletionPercent { get { return _completionPercent; } }
        public uint CupId { get { return _cupId; } }
        public uint TrackIndex { get { return _trackIndex; } }
        public List<uint> Points { get { return _points; } }
        public byte[] StandingPoints { get { return _standingPoints; } }
        public uint CurrLev { get { return _currLev; } }
        public uint PrevLev { get { return _prevLev; } }
        public uint BossId { get { return _bossId; } }
        public uint ArcadeDifficulty { get { return _arcadeDifficulty; } }
        public uint NumMissiles { get { return _numMissiles; } }
        public uint NumPlayersWith3Misiles { get { return _numPlayersWith3Misiles; } }
        public uint RainVar { get { return _rainVar; } }
        public uint PtrRedOff { get { return _ptrRedOff; } }
        public uint PtrRedOn { get { return _ptrRedOn; } }
        public uint PtrGreenOff { get { return _ptrGreenOff; } }
        public uint PtrGreenOn { get { return _ptrGreenOn; } }
        public uint DemoCountdownTimer { get { return _demoCountdownTimer; } }
        public uint Unk1Afterdemo { get { return _unk1Afterdemo; } }
        public uint Unk2Afterdemo { get { return _unk2Afterdemo; } }
        public uint Unk3Afterdemo { get { return _unk3Afterdemo; } }
        public List<uint> PtrIcons { get { return _ptrIcons; } }
        public uint Unk210c { get { return _unk210c; } }
        public uint Unk2110 { get { return _unk2110; } }
        public List<uint> IconGroup { get { return _iconGroup; } }
        public uint Unk2158 { get { return _unk2158; } }
        public uint Unk215c { get { return _unk215c; } }
        public List<uint> ModelPtr { get { return _modelPtr; } }
        public List<uint> PtrDrivers { get { return _ptrDrivers; } }
        public List<uint> PtrDriversOrdered { get { return _ptrDriversOrdered; } }
        public Deadcoed DeadcoedStruct { get { return _deadcoedStruct; } }
        public byte[] FillerNull1 { get { return _fillerNull1; } }
        public byte OverlayLoaded1 { get { return _overlayLoaded1; } }
        public byte OverlayLoaded2 { get { return _overlayLoaded2; } }
        public byte OverlayLoaded3Neverused { get { return _overlayLoaded3Neverused; } }
        public byte OverlayLoaded4 { get { return _overlayLoaded4; } }
        public byte[] SkipFinalFiller { get { return _skipFinalFiller; } }
        public uint FrameDelayMaybe { get { return _frameDelayMaybe; } }

        /// <summary>
        /// 0x256c - uint - render flags
        /// 
        /// 00000001 - draw lev
        /// 00000002 - draw rain
        /// 00000004 - ?
        /// 00000008 - draw stars
        /// 00000010 - ?
        /// 00000020 - draw ctr models (instances?)
        /// 00000040 - ?
        /// 00000080 - probably wheels, but doesn't render without kart
        /// 00000100 - ?
        /// 00000200 - draw particles (fire, smoke)
        /// 00000400 - draw shadow
        /// 00000800 - draw heat effect
        /// 00001000 - trigger checkered flag
        /// 00002000 - clear back buffer with back color
        /// 00004000 - ?
        /// 00008000 - ?
        /// 
        /// rest unknown or no visible effects
        /// </summary>
        public uint Renderflags { get { return _renderflags; } }

        /// <summary>
        /// only bit0 has effect
        /// </summary>
        public ushort ClockEffectEnabled { get { return _clockEffectEnabled; } }
        public ushort ValueAfterClockEffect { get { return _valueAfterClockEffect; } }
        public List<byte> SomeHowlBankData { get { return _someHowlBankData; } }
        public byte SomeValueBeforePositions1 { get { return _someValueBeforePositions1; } }
        public byte SomeValueBeforePositions2 { get { return _someValueBeforePositions2; } }
        public List<byte> CurrentHumanPlayerPosition { get { return _currentHumanPlayerPosition; } }
        public short FinalShort { get { return _finalShort; } }
        public CtrGameconfig M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
