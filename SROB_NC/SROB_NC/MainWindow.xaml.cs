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

            // Get the Min- / Max-Position
            SoftwareMin = ParameterCollecion.GetPoint4D("PAR_SW_ES_MIN");
            SoftwareMax = ParameterCollecion.GetPoint4D("PAR_SW_ES_MAX");

            SelectionShutters.AddRange(Config.Shutters.ShutterList);

            //Load Shutters to Binding Property
            ShutterSelection.ItemsSource = SelectionShutters;
            ShutterSelection.DisplayMemberPath = "Name";
            ShutterSelection.SelectedValuePath = "Key";
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

        public List<Shutter> SelectionShutters = new List<Shutter>();

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

                OnPropertyChanged("PosX");
                OnPropertyChanged("PosY");
                OnPropertyChanged("PosZ");
                OnPropertyChanged("PosC");

                MoveObject(value, _viewport.MovingBody);
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
            Config.Initialize(Environment.CurrentDirectory + "/../../../");

            TrackValid = false;
            ResultSweep = -1;

            _viewport.Initialize();
            _track.Waypoints.Clear();

            CurrentPos = CurrentPos;
        }
        #endregion

        #region CalcStart Click
        private void CalcStart_Click(object sender, RoutedEventArgs e)
        {
            Refresh_Click(sender, e);

            _track.Waypoints.Add(new Point_4D(5000, 4200, 500, 0));
            //CurrentPos = new Point_4D(1000, 1000, 100, 90);

            _track.Waypoints.Add(new Point_4D(CurrentPos));

            Geometries.Size movingSize;
            //movingSize.Length = Config.Params.Values["GRIPPER_DIM[0]"];
            //movingSize.Width = Config.Params.Values["GRIPPER_DIM[1]"];
            movingSize.Length = 1000;
            movingSize.Width = 100;
            movingSize.Height = 100;

            TrackValid = _track.Solve(movingSize, out List<Point_4D> result, relevantAreas: Config.ResAreas.Areas);

            if (TrackValid)
            {
                _viewport.AddTrack(result);

                foreach (var point in result)
                {
                    _viewport.AddMidPosition(point, movingSize);
                }

                _viewport.RedrawTransparants();
            }

        }
        #endregion

        #endregion  
    }
}
