using System;
using System.Linq;
using System.Reflection;
using MRL.IDE.Base;
using MRL.Utils;

namespace MRL.IDE.Robot
{
    public static class ValidatedRobot
    {
        public enum RobotType { P3AT, Kenaf, AirRobot, Nao, ARDrone, TeleMax, pioneer3at_with_sensors} // based on rull 2012

        public static Type[] GetRobotTypes()
        {
            var ns = typeof(ValidatedRobot).Namespace + '.';
            return (from x in Enum.GetNames(typeof(RobotType))
                    let t = Assembly.GetExecutingAssembly().GetType(ns + x)
                    where t != null && t.BaseType == typeof(BaseRobot)
                    select t).ToArray();
        }

        private static Type GetRobotType(string type)
        {
            return GetRobotTypes().Where(x => x.Name.ToLower() == type.ToLower()).FirstOrDefault();
        }

        public static BaseRobot GetObject(RobotInfo ri, int mSpawnIndex)
        {
            var type = GetRobotType(ri.Type);
            if (type == null)
                return null;
            return (BaseRobot)Activator.CreateInstance(type, ri, mSpawnIndex);
        }
    }
}
