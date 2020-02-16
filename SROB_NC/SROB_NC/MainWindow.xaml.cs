using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Configuration;
using Komm;
using Geometries;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace SROB_NC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constructors
        public MainWindow()
        {
            Config.Initialize(Environment.CurrentDirectory + "/../../../");

            InitializeComponent();

            this.DataContext = this;

            CurrentPos = new T_P_4D(0, 0, 0, 0);

            #region Initialize Config

            #endregion

        }

        #endregion

        #region Properties
        private bool _ADSonline;
        public bool ADS_Online
        {
            get => _ADSonline;
            set => _ADSonline = ConnectToPLC(value);
        }


        #region CurrentPos

        public T_P_4D CurrentPos
        {
            get => GetPropertyValue<T_P_4D>();
            set
            {
                SetPropertyValue(value);
                OnPropertyChanged("PosX");
                OnPropertyChanged("PosY");
                OnPropertyChanged("PosZ");
                OnPropertyChanged("PosC");

                MoveObject(value, _viewport.Gripper);
            }
        }

        public string PosX { get => $"X {CurrentPos.X:0.0}"; }
        public string PosY { get => $"Y {CurrentPos.Y:0.0}"; }
        public string PosZ { get => $"Z {CurrentPos.Z:0.0}"; }
        public string PosC { get => $"C {CurrentPos.C:0.0}"; }

        #endregion

        #endregion

        #region Methods

        #region ConnectToPLC
        /// <summary>
        /// Changes connection State of PLC Connection and returns accomplished state.
        /// </summary>
        /// <param name="state">state to be set</param>
        /// <returns></returns>
        private bool ConnectToPLC(bool state)
        {
            if (state == ADS_Online)
                return state;

            try
            {
                if (state)
                {
                    if (Config.Ini.Values["ADS_Port"].Length > 0)
                    {
                        PLC.Connect("", int.Parse(Config.Ini.Values["ADS_Port"]));

                        //Establish notifications
                        PLC.CreateNewNoty(".Visu.Module[0].Position", SetPose_OnADSNotification);

                        return true;
                    }
                }

                else
                    PLC.DISCOnnect();

            }
            catch (Exception ex)
            {
                Console.WriteLine("PLC-Connection not possible");
            }

            return false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        #region Fields

        /// <summary>
        /// The backing Directory for the Properties.
        /// </summary>
        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();

        #endregion

        #region Methods

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Gets the Property value from the Dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the Property.</typeparam>
        /// <param name="propertyName">The Property name (optional).</param>
        /// <returns>The Property value.</returns>
        protected T GetPropertyValue<T>([CallerMemberName] string propertyName = null)
        {
            try
            {
                if (propertyName == null) throw new ArgumentNullException("propertyName");

                object value;
                if (_propertyBackingDictionary.TryGetValue(propertyName, out value))
                {
                    return (T)value;
                }

                return default(T);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error at GetPropertyValue: {0}", ex.Message));
                return default(T);
            }
        }

        /// <summary>
        /// Sets the Property value.
        /// </summary>
        /// <typeparam name="T">The type of the Property.</typeparam>
        /// <param name="newValue">The new value to set.</param>
        /// <param name="propertyName">The Property name (optional).</param>
        /// <returns>True if it was set successfully, otherwise false.</returns>
        protected bool SetPropertyValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            try
            {
                if (propertyName == null) throw new ArgumentNullException("propertyName");

                if (EqualityComparer<T>.Default.Equals(newValue, GetPropertyValue<T>(propertyName))) return false;

                _propertyBackingDictionary[propertyName] = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error at SetPropertyValue: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Checks if it's possible to execute the GlobalObjects.BefehleModelBefehl.
        /// </summary>
        /// <returns></returns>
        public bool CanExecute()
        {
            return true;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region MoveObject
        /// <summary>
        /// Moves the given ocject to specified position in 3D-Space
        /// </summary>
        /// <param name="pose">4D-position to be moved to</param>
        /// <param name="obj">Object of type <see cref="ModelVisual3D"/> to be moved</param>
        private void MoveObject(T_P_4D pose, ModelVisual3D obj)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Translate(new Vector3D(pose.X, pose.Y, pose.Z));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), pose.C), new Point3D(pose.X, pose.Y, pose.Z));

            _viewport.Dispatcher.Invoke(new Action(() => obj.Transform = new MatrixTransform3D(matrix)));
        }
        #endregion

        #endregion

        #region Events

        #region SetPose_OnADSNotification
        private void SetPose_OnADSNotification(System.IO.Stream posData)
        {
            var binReader = new System.IO.BinaryReader(posData);

            CurrentPos.X = binReader.ReadSingle();
            binReader.ReadSingle();

            CurrentPos.Y = binReader.ReadSingle();
            binReader.ReadSingle();

            CurrentPos.Z = binReader.ReadSingle();
            binReader.ReadSingle();

            CurrentPos.C = binReader.ReadSingle();
            binReader.ReadSingle();

            CurrentPos = CurrentPos;
        }
        #endregion

        #region Label OnMousWheel
        private void target_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {

            float distance = 50 * (e.Delta > 0 ? 1 : -1);

            switch (((Label)sender).Name)
            {
                case "PoseX":
                    CurrentPos.X += distance;
                    break;

                case "PoseY":
                    CurrentPos.Y += distance;
                    break;

                case "PoseZ":
                    CurrentPos.Z += distance;
                    break;

                case "PoseC":
                    CurrentPos.C += 10 * (e.Delta > 0 ? 1 : -1); ;
                    break;
            }

            CurrentPos = CurrentPos; //To call Setter
        }

        #endregion

        #region ToggleSwitch Click

        private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            
        }

        #endregion

        #endregion

    }
}
