using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BezierDRAW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentMode = "";
        private int clickCounter = 0;
        private Point startPoint, endPoint;
        private Point startBezierPoint, endBezierPoint, firstControlBezierPoint, secondControlBezierPoint;
        private Point[] myPoints;
        private string[] allModes = { "handline", "straigthline", "bezierline" };
        private Line line, firstline;
        private DoubleCollection dottedLine;

        public MainWindow()
        {
            dottedLine = new DoubleCollection();
            dottedLine.Add(4);
            dottedLine.Add(2);
            InitializeComponent();
        }

        private void DeleteIfNeeded()
        {
            if (line != null)
                myImage.Children.Remove(line);
            if (myPoints != null)
            {
                for (int i = 0; i < myPoints.Length - 1; i++)
                    myImage.Children.RemoveAt(0);
            }
        }
        private void DrawStraigthLine(Point p1, Point p2, int color, bool ifDotted)
        {
            line = new Line();

            if (color == 2) line.Stroke = Brushes.Green;
            else line.Stroke = Brushes.Black;

            if (ifDotted)
                line.StrokeDashArray = dottedLine;

            line.X1 = p1.X;
            line.X2 = p2.X;
            line.Y1 = p1.Y;
            line.Y2 = p2.Y;

            line.StrokeThickness = 3;
            myImage.Children.Add(line);
            
        }
        private void DrawMyBezierCurve()
        {
            MyBezierCurve.CalculateMyBezier(0.00390625, startBezierPoint, firstControlBezierPoint, secondControlBezierPoint, endBezierPoint);
            myPoints = MyBezierCurve.GetMyBezierPoints().ToArray();
            for(int i=0; i < myPoints.Length -1; i++)
            {
                Line lineB = new Line();
                lineB.Stroke = Brushes.Black;
                lineB.StrokeThickness = 3;
                lineB.X1 = myPoints[i].X;
                lineB.X2 = myPoints[i+1].X;
                lineB.Y1 = myPoints[i].Y;
                lineB.Y2 = myPoints[i+1].Y;
                myImage.Children.Insert(0,lineB);
            }
        }

        private void myImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentMode == "handline")
            {
                DrawStraigthLine(startPoint, e.GetPosition(myImage), 3, false);
                startPoint = e.GetPosition(myImage);
            }
            else if ((currentMode == "straigthline" || currentMode == "bezierline") && clickCounter == 1) 
            {

                if (e.LeftButton == MouseButtonState.Pressed) return;

                DeleteIfNeeded();
                DrawStraigthLine(startPoint, e.GetPosition(myImage), 3, false);

            }
            else if (currentMode == "bezierline" && clickCounter == 2)
            {
                if (e.LeftButton == MouseButtonState.Pressed) return;

                DeleteIfNeeded();
                endBezierPoint = e.GetPosition(myImage);
                secondControlBezierPoint = e.GetPosition(myImage);
                DrawMyBezierCurve();
            }
            else if(currentMode == "bezierline" && clickCounter == 3)
            {
                if (e.LeftButton == MouseButtonState.Pressed) return;
                DeleteIfNeeded();
                secondControlBezierPoint = e.GetPosition(myImage);
                DrawStraigthLine(endBezierPoint, e.GetPosition(myImage), 2, true);
                DrawMyBezierCurve();
            }
        }

        private void myImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == "handline")
                startPoint = e.GetPosition(myImage);
        }
        private void myImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (currentMode == "")
                myStatusText.Text = "Najpierw wybierz tryb!";
            else if (currentMode == "straigthline")
            {
                clickCounter++;
                if (clickCounter == 1)
                {
                    myStatusText.Text = "Wybrano tryb rysowania prostych odcinków. Podaj punkt końcowy.";
                    startPoint = e.GetPosition(myImage);
                }
                else if (clickCounter == 2)
                {
                    clickCounter = 0;
                    myStatusText.Text = "Wybrano tryb rysowania prostych odcinków. Podaj punkt początkowy.";
                    line = null;
                }
            }
            else if (currentMode == "bezierline")
            {
                clickCounter++;
                if (clickCounter == 1)
                {
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj pierwszy punkt kontrolny.";
                    startBezierPoint = e.GetPosition(myImage);
                    startPoint = startBezierPoint;
                }
                else if (clickCounter == 2)
                {
                    DeleteIfNeeded();
                    endPoint = e.GetPosition(myImage);
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj punkt końcowy.";
                    firstControlBezierPoint = e.GetPosition(myImage);
                    firstline = new Line();
                    firstline.X1 = startBezierPoint.X;
                    firstline.X2 = firstControlBezierPoint.X;
                    firstline.Y1 = startBezierPoint.Y;
                    firstline.Y2 = firstControlBezierPoint.Y;
                    firstline.Stroke = Brushes.Red;
                    firstline.StrokeThickness = 3;
                    firstline.StrokeDashArray = dottedLine;
                    myImage.Children.Add(firstline);

                }
                else if (clickCounter == 3)
                {
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj drugi punkt kontrolny.";
                    endBezierPoint = e.GetPosition(myImage);
                    secondControlBezierPoint = e.GetPosition(myImage);
                }
                else if (clickCounter == 4)
                {
                    DeleteIfNeeded();
                    secondControlBezierPoint = e.GetPosition(myImage);
                    
                    DrawStraigthLine(endBezierPoint, secondControlBezierPoint, 2, true);
                    DrawMyBezierCurve();
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj punkt początkowy.";
                    clickCounter = 0;
                    myPoints = null;
                    line = null;
                    firstline = null;
                }
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            handlineButton.BorderBrush = Brushes.Black;
            handlineButton.BorderThickness = new Thickness(1);
            straigthlineButton.BorderBrush = Brushes.Black;
            straigthlineButton.BorderThickness = new Thickness(1);
            bezierlineButton.BorderBrush = Brushes.Black;
            bezierlineButton.BorderThickness = new Thickness(1);
            Button mybtn = (Button)sender;
            if (mybtn.Name != "clearButton")
            {
                mybtn.BorderBrush = Brushes.Blue;
                mybtn.BorderThickness = new Thickness(3);
            }
            clickCounter = 0;
            DeleteIfNeeded();
            if (line != null)
                myImage.Children.Remove(firstline);
            myPoints = null;
            line = null;
            switch (mybtn.Name)
            {
                case "handlineButton":
                    currentMode = "handline";
                    myStatusText.Text = "Wybrano tryb rysowania odręcznego.";
                    break;
                case "straigthlineButton":
                    currentMode = "straigthline";
                    myStatusText.Text = "Wybrano tryb rysowania prostych odcinków. Podaj punkt początkowy.";
                    break;
                case "bezierlineButton":
                    currentMode = "bezierline";
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj punkt początkowy.";
                    break;
                case "clearButton":
                    currentMode = "";
                    myImage.Children.Clear();
                    myStatusText.Text = "Witaj w programie BezierDRAW! Wybierz jeden spośród trzech trybów aby rozpocząć rysowanie.";
                    break;
                case "exitButton":
                    Application.Current.Shutdown();
                    break;
            }
        }
    }
}