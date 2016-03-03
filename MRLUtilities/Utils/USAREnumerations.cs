
namespace MRL.Utils
{

    public enum PreliminaryTestMode
    {
        MAPPING_MODE = 0,
        DEPLOYMENT_MODE = 1,
        TELEOPERATION_MODE = 2,
        COMPREHENSIVE_MODE = 3
    }

    /// <summary>
    /// used in CommunicationGraph ( model of wireless comstation )
    /// </summary>
    public enum WirelessPropagationModel
    {
        DISTANCE_PROPAGATION_MODEL = 0,
        OBSTACLE_PROPAGATION_MODEL = 1
    }

    /// <summary>
    /// used in PathFinder
    /// </summary>
    public enum eRoadState
    {
        FAILURE = 200,
        SUCCESS
    };

    /// <summary>
    /// configuration of layer index of widgets 
    /// </summary>
    public enum WidgetPriority
    {
        MAP,
        MISSION,
        ROBOTPATH,
        ANNOTATION,
        CAMERAIMAGE,
        VICTIM,
        RSSI,
        GOALPOINTS,
        ROBOT,
        SCALEBAR,
        COMSTATION
    }

    /// <summary>
    /// used in ComStation->AddVictim function
    /// </summary>
    public enum VictimCollectionState
    {
        ADD,
        UPDATE,
        EXPEL
    }

    /// <summary>
    /// Autonomous strategy modes
    /// </summary>
    public enum AutonomyStrategy
    {
        FRONTIER
    }

    /// <summary> 
    /// control mode : 
    /// 1> PID control 
    /// 2> two speed control
    /// 3> constant
    /// </summary>
    public enum WheelControlMode
    {
        WCM_PID = 1,
        WCM_TwoSpeed,
        WCM_Constant
    }

    /// <summary>
    /// Console Message Type
    /// </summary>
    public enum ConsoleMessageType
    {
        Normal = 0,
        Error = 1,
        Exclamation = 2,
        Information = 3
    }

    /// <summary>
    /// Teleoperation Driving Types
    /// </summary>
    public enum DriveType
    {
        MoveStop = -3,
        MoveInSide = -2,
        Fly = -1,
        Straight = 0,
        Rotate = 2,
        Light = 4,
        MoveArm_All = 5,
        MoveArm_Front = 6,
        MoveArm_Rear = 7,
        MoveArm_fl = 8,
        MoveArm_fr = 9,
        MoveArm_rl = 10,
        MoveArm_rr = 11
    }

    /// <summary>
    /// Value Types
    /// </summary>
    public enum UnitType
    {
        UNIT_RAW = 0,
        UNIT_M = 1,
        UNIT_CM = 2,
        UNIT_MM = 3,
        UNIT_RAD = 4,
        UNIT_DEG = 5,
        UNIT_UU = 6
    }

    /// <summary>
    /// Robot's Decision States
    /// </summary>
    public enum MachineState
    {
        STA_STOP = 0,
        STA_EXPLORATION,
        STA_ROTATE,
        STA_MOVE,
        STA_DANGER,
        STA_RECOVER,
        STA_AVOID,
        STA_JOYPAD,
        STA_BEGINMISSION,
        STA_CHOOSEMISSION,
        STA_DOMISSION,
        STA_FINISHMISSION,
        STA_LEADERFOLLOWING,
        STA_DISCONNECTED,
        STA_LOCK
    };

    /// <summary>
    /// mode: 1: ground truth
    /// mode: 2: odometry
    /// mode: 3: odometry+scan_match
    /// mode: 4: INS
    /// mode: 5: INS+scan_match
    /// </summary>
    public enum LocalizationMode
    {
        LM_GROUNDTRUTH = 0,
        LM_RAW_ODOMETRY = 1,
        LM_ODO_SLAM = 2,
        LM_RAW_INS = 3,
        LM_INS_SLAM = 4
    }

    /// <summary> 
    /// This is how important this message is to the effective functioning of the team.
    /// Since it is set by the creator of a message, there are no enforced semantics,
    /// but the following interpretations are encouraged:
    /// 
    /// HIGH: This message must get through, communication should do everything it can to
    /// get it through.
    /// 
    /// MEDIUM: Default level, coordination is going to break if this message doesn't get through,
    /// but in a fixable and non-catastrophic way.  Communication should really endeavour to get it
    /// through.
    /// 
    /// LOW: More routine message, coordination needs this message to get through but likely things
    /// will work fine even if it doesn't get through.  Communication should try to get the message
    /// through, but prioritize other things.
    /// 
    /// NONE: Doesn't matter too much whether this gets through or not.  Of course, we wouldn't be
    /// sending it if was completely useless but don't stress about getting it through.
    /// </summary>
    public enum MessagePriority
    {
        INP_MSG_NONE = 0,
        INP_MSG_LOW = 1,
        INP_MSG_MEDIUM = 2,
        INP_MSG_HIGH = 3
    }

    /// <summary>
    /// Robot's Link Type
    /// 0 - Command Link (can be image,string or file)
    /// 1 - Video Link (only use for sending camera images)
    /// </summary>
    public enum RobotLinkType
    {
        COMMAND_LINK = 0,
        VIDEO_LINK = 1
    }

    /// <summary>
    /// Internal & External Packet Types
    /// </summary>
    public enum PacketType
    {
        STRING_PACKET = 0,
        BITMAP_PACKET = 1,
        FILE_PACKET = 2,
        ROUTED_PACKET = 3
    }

    /// <summary>
    /// id for an internal message
    /// </summary>
    public enum InternalMessagesID : byte
    {
        Battery, Position3D, RangeScan, PortInfo, ImageServer, Drive,
        Mission, VictimRFID, CommunicationGraph, CameraImage, AutonomousChange,
        RoutedMessage,
        RequestDVGraph, IMNode, IADATA,
        JoyStickData, FrontierList, PressedKeys
    }

    public enum ImageTransferType
    {
        DIRECT = 1, NETWORK
    }
    public enum WidgetTypes
    {
        Map = 12,
        Robot = 2,
        ComStation = 3,
        GoalPoint = 4,
        CameraImage = 5,
        RobotPath = 6,
        Victim = 7,
        Annotation = 8,
        ScaleBar = 1,
        Mission = 10,
        ComGraph = 11,
        RSSI = 12,
        lines = 19,
        SignalWidget

    }

    public enum MissionState
    {
        IDLE,
        WAIT,
        SELECT,
        RECOVER,
        GOTO,
        OBS_AVOIDANCE
    }

    public enum ObstacleAlgorthm
    { 
        MOTION,
        SINGL_POINT
    }

    public enum ObstacleStatus
    {
        OBSTACLE,
        CLEAR,
        FAILD,
        TRY
    }
}
