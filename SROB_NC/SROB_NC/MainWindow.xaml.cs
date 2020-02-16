﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Configuration;
using Komm;
using Geometries;
using System.ComponentModel;

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

            #region Initialize Config
            #endregion

            #endregion
        }

        #region Properties
        private bool _ADSonline;
        public bool ADS_Online
        {
            get => _ADSonline;
            set => ConnectToPLC(value);
        }


        #region CurrentPos
        private T_P_4D _currentPos = new T_P_4D();
        public T_P_4D CurrentPos
        {
            get => _currentPos;
            set
            {
                _currentPos = value;
                OnPropertyChanged("PosX");
                OnPropertyChanged("PosY");
                OnPropertyChanged("PosZ");
                OnPropertyChanged("PosC");

                MoveObject(value, _viewport.Gripper);
            }
        }

        public string PosX { get => $"X {_currentPos.X:0.#}"; }
        public string PosY { get => $"Y {_currentPos.Y:0.#}"; }
        public string PosZ { get => $"Z {_currentPos.Z:0.#}"; }
        public string PosC { get => $"C {_currentPos.C:0.#}"; }

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
            catch (Exception)
            {
                Console.WriteLine("PLC-Connection not possible");
            }

            return false;
        }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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