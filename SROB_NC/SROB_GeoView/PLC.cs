using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace Komm
{
    public static class PLC
    {
        private static TcAdsClient _adsClient;
        private static List<int> _handles = new List<int>();
        private static Dictionary<int, Action<float>> _floatActions = new Dictionary<int, Action<float>>();
        private static Dictionary<int, Action<int>> _intActions = new Dictionary<int, Action<int>>();
        private static Dictionary<int, Action<bool>> _boolActions = new Dictionary<int, Action<bool>>();
        private static Dictionary<int, Action<AdsStream>> _streamActions = new Dictionary<int, Action<AdsStream>>();

        //   private static Dictionary<int, Action<float>> _
        public static bool Connect(string aMSNetID, int port)
        {
            try
            {
                _adsClient = new TcAdsClient();
                _adsClient.Connect(aMSNetID, port);
                _adsClient.AdsNotificationEx += _adsClient_AdsNotificationEx;
                _adsClient.AdsNotification += _adsClient_AdsNotification;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ADS: " + e.Message);
            }
            return false;
        }

        public static void DISCOnnect()
        {
            foreach (var h in _handles)
            {
                _adsClient.DeleteDeviceNotification(h);
            }
            _adsClient.Close();
        }


        public static int CreateNewNoty(string varName, Action<AdsStream> action)
        {
            int handle = _adsClient.AddDeviceNotification(varName, new AdsStream(_adsClient.ReadSymbolInfo(varName).Size),
                AdsTransMode.OnChange, 100, 100, null);

            if (!_streamActions.ContainsKey(handle))
                _streamActions.Add(handle, action as Action<AdsStream>);

            return handle;
        }

        private static void _adsClient_AdsNotification(object sender, AdsNotificationEventArgs e)
        {
            if (_streamActions.ContainsKey(e.NotificationHandle))
                _streamActions[e.NotificationHandle](e.DataStream);

            else
                Console.WriteLine($"Unknown Notification handle: {e.NotificationHandle}");

        }

        public static int CreateNewNoty<T>(string varName, Action<T> action) where T : struct
        {
            int handle = _adsClient.AddDeviceNotificationEx(varName, AdsTransMode.OnChange, 100, 100, null, typeof(T));
            _handles.Add(handle);

            switch (action)
            {
                case Action<float> a:
                    _floatActions.Add(handle, action as Action<float>);
                    break;

                case Action<int> a:
                    _intActions.Add(handle, action as Action<int>);
                    break;

                case Action<bool> a:
                    _boolActions.Add(handle, action as Action<bool>);
                    break;

                default:
                    Console.WriteLine($"Non supported Type: {typeof(T).Name}");
                    break;
            }

            return handle;
        }

        private static void _adsClient_AdsNotificationEx(object sender, AdsNotificationExEventArgs e)
        {
            if (_floatActions.ContainsKey(e.NotificationHandle))
                _floatActions[e.NotificationHandle]((float)e.Value); //execute stored action

            else if (_intActions.ContainsKey(e.NotificationHandle))
                _intActions[e.NotificationHandle]((int)e.Value);

            else
            {
                Console.WriteLine($"Unknown Notification handle: {e.NotificationHandle}");
            }

        }


        public static AdsStream ReadStream(string varName, int length)
        {
            try
            {
                var h = _adsClient.CreateVariableHandle(varName);
                var s = new AdsStream(length);
                _adsClient.Read(h, s, 0, length);
                _adsClient.DeleteVariableHandle(h);
                return s;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static float ReadValue(string varName)
        {
            float o;
            var h = _adsClient.CreateVariableHandle(varName);

            AdsStream s;
            AdsBinaryReader r;

            s = new AdsStream(32);
            r = new AdsBinaryReader(s);
            _adsClient.Read(h, s, 0, s.Capacity);
            o = r.ReadSingle();

            _adsClient.DeleteVariableHandle(h);

            return o;
        }
    }
}
