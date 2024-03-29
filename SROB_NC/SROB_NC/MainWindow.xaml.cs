﻿using System;
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
            Config.Initialize();

            InitializeComponent();

            this.DataContext = this;

            // Get the Min- / Max-Position
            SoftwareMin = ParameterCollecion.GetPoint4D("PAR_SW_ES_MIN");
            SoftwareMax = ParameterCollecion.GetPoint4D("PAR_SW_ES_MAX");

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

        private readonly Track _track = new Track();

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

        #region Sweep along path
        private int _resultSweep = -1;
        public int ResultSweep
        {
            get { return _resultSweep; }
            set
            {
                if (_resultSweep == -1)
                    _viewport.Children.Remove(_viewport.MidPositions);

                _resultSweep = value;

                if (_resultSweep < 0)
                    return;

                //Get Segment Current on
                var currentTravel = _track.Length / 100 * value;

                double segmentSum = 0;
                Segment_4D currentSegment = null;

                foreach (var segment in _track.ResultSegments)
                {
                    segmentSum += segment.Length;

                    if (segmentSum > currentTravel)
                    {
                        currentSegment = segment;

                        //now calculate from last MidPoint
                        currentTravel -= (segmentSum - segment.Length);
                        break;
                    }
                }

                if (currentSegment != null)
                    CurrentPos = currentSegment.GetPositionAt(Axis.None, currentTravel);

            }
        }

        private bool _trackValid;

        public bool TrackValid
        {
            get { return _trackValid; }
            set
            {
                _trackValid = value;
                OnPropertyChanged("TrackValid");
            }
        }
        #endregion

        #region CurrentPos
        private Point_4D _currentPos = new Point_4D();

        /// <summary>
        /// The current Position of the Gripper.
        /// </summary>
        public Point_4D CurrentPos
        {
            get => _currentPos;
            set
            {
                _currentPos = Point_4D.Max(SoftwareMin, value);
                _currentPos = Point_4D.Min(SoftwareMax, _currentPos);

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

        /// <summary>
        /// Maximum Position (from Parameters)
        /// </summary>
        public Point_4D SoftwareMax { get; set; }

        /// <summary>
        /// Minimum Position (from Parameters)
        /// </summary>
        public Point_4D SoftwareMin { get; set; }

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
        private void MoveObject(Point_4D pose, ModelVisual3D obj)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Translate(new Vector3D(pose.X, pose.Y, pose.Z));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), pose.C), new Point3D(pose.X, pose.Y, pose.Z));

            _viewport.Dispatcher.Invoke(new Action(() => obj.Transform = new MatrixTransform3D(matrix)));
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

            TrackValid = false;
            ResultSweep = -1;

            btnCalcStart.Content = "Set Start";

            _viewport.Initialize();
            UpdateMovingBody(SelectedShutter);
            _track.Waypoints.Clear();

            CurrentPos = CurrentPos;
        }
        #endregion

        #region Reload Click

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            Config.Reset();

            Refresh_Click(sender, e);
        }

        #endregion

        #region CalcStart Click
        private void CalcStart_Click(object sender, RoutedEventArgs e)
        {
             _track.Waypoints.Add(new Point_4D(CurrentPos));
            if (_track.Waypoints.Count < 2)
            {
                btnCalcStart.Content = "Solve Track";
                _viewport.AddMidPosition(CurrentPos, SelectedShutter.Size);
                _viewport.RedrawTransparants();
            }
            else
            {
                if (_track.Waypoints.Count > 2)
                {
                    _track.Waypoints.RemoveAt(1);
                    _viewport.Initialize();
                    UpdateMovingBody(SelectedShutter);
                    CurrentPos = CurrentPos;

                    ResultSweep = -1;
                }

                if (_track.Solve(SelectedShutter.Size, out List<Point_4D> result, relevantAreas: Config.ResAreas.Areas))
                {
                    _viewport.AddTrack(result);

                    result.ForEach(point => _viewport.AddMidPosition(point, SelectedShutter.Size));

                    _viewport.RedrawTransparants();

                    TrackValid = true;
                }
            }
        }
        #endregion

        #endregion

    }
}
