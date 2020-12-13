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
using System.Collections.ObjectModel;
using Configuration.Parameters;
using Configuration.Shutters;

namespace SROB_3DViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constructors
        public MainWindow()
        {
            Config.Initialize();

            InitializeComponent();

            this.DataContext = this;

            SelectionShutters = new ObservableCollection<Shutter>();
            foreach (var item in Config.Shutters.ShutterList)
            {
                SelectionShutters.Add(item);
            }

            SelectedShutter = new Shutter
            {
                Key = 0,
                Length = Config.Params.Values["GREIFER_KOPF_L_GES"],
                Width = Config.Params.Values["GREIFER_KOPF_B_GES"],
                Height = 300,
            };

        }
        #endregion

        #region Properties
        private bool _ADSonline;
        public bool ADS_Online
        {
            get => _ADSonline;
            set => _ADSonline = ConnectToPLC(value);
        }

        #region Shutters - Collection

        public ObservableCollection<Shutter> SelectionShutters { get; set; }

        #endregion

        #region Selected Shutter

        private Shutter _selectedShutter;
        public Shutter SelectedShutter
        {
            get => _selectedShutter;
            set 
            { 
                _selectedShutter = value;
                OnPropertyChanged("SelectedShutter");

                UpdateMovingBody(value);
            }
        }

        #endregion

        #region CurrentPos
        private Pnt4D _currentPos = new Pnt4D();

        /// <summary>
        /// The current Position of the Gripper.
        /// </summary>
        public Pnt4D CurrentPos
        {
            get => _currentPos;
            set
            {
                OnPropertyChanged("CurrentPos");

                OnPropertyChanged("PosX");
                OnPropertyChanged("PosY");
                OnPropertyChanged("PosZ");
                OnPropertyChanged("PosC");
            }
        }

        public string PosX { get => $"X {_currentPos.X:0.0}"; }
        public string PosY { get => $"Y {_currentPos.Y:0.0}"; }
        public string PosZ { get => $"Z {_currentPos.Z:0.0}"; }
        public string PosC { get => $"C {_currentPos.C:0.0}"; }

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
                    //if (Config.Ini.Values["ADS_Port"].Length > 0)
                    //{
                    //    PLC.Connect("", int.Parse(Config.Ini.Values["ADS_Port"]));

                    //    //Establish notifications
                    //    PLC.CreateNewNoty(".Visu.Module[0].Position", SetPose_OnADSNotification);

                    //    return true;
                    //}
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Update the MovingBody
        private void UpdateMovingBody(Shutter value)
        {
            if (value == null)
                return;

            //_viewport.MovingBody.Length = value.Length;
            //_viewport.MovingBody.Height = value.Height;
            //_viewport.MovingBody.Width = value.Width;
            //_viewport.MovingBody.Center = new Point3D(0,0,value.Height/2);
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
        private void Target_OnMouseWheel(object sender, MouseWheelEventArgs e)
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

        #region Refresh Click
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Config.Initialize();
            _viewport.Initialize();
        }
        #endregion

        #region Reload Click

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            Config.Reset();

            Refresh_Click(sender, e);
        }

        #endregion

        #endregion

    }
}
