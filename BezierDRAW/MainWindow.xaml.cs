using System.Threading;
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
        private Line line;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawStraigthLine(Point p1, Point p2)
        {
            line = new Line();
            line.Stroke = Brushes.Black;
            line.X1 = p1.X;
            line.X2 = p2.X;
            line.Y1 = p1.Y;
            line.Y2 = p2.Y;
            line.StrokeThickness = 3;
            myImage.Children.Add(line);
            
        }

        private void DrawMyBezierCurve()
        {
            MyBezierCurve.CalculateMyBezier(0.0078125, startBezierPoint, firstControlBezierPoint, secondControlBezierPoint, endBezierPoint);
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
                line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = e.GetPosition(myImage).X;
                line.Y2 = e.GetPosition(myImage).Y;
                startPoint = e.GetPosition(myImage);
                line.StrokeThickness = 3;
                myImage.Children.Add(line);
            }
            else if ((currentMode == "straigthline" || currentMode == "bezierline") && clickCounter == 1) 
            {
                
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Thread.Sleep(100);
                    if (currentMode == "straigthline")
                    {
                        clickCounter = 0;
                        myStatusText.Text = "Wybrano tryb rysowania prostych odcinków. Podaj punkt początkowy.";
                        line = null;
                    }
                    else
                    {
                        endPoint = e.GetPosition(myImage);
                        myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj punkt końcowy.";
                        firstControlBezierPoint = e.GetPosition(myImage);
                        clickCounter = 2;
                    }
                    return;
                }

                if (line!=null)
                    myImage.Children.Remove(line);

                line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = e.GetPosition(myImage).X;
                line.Y2 = e.GetPosition(myImage).Y;
                line.StrokeThickness = 3;
                myImage.Children.Add(line);

            }
            else if (currentMode == "bezierline" && clickCounter == 2)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj drugi punkt kontrolny.";
                    endBezierPoint = e.GetPosition(myImage);
                    secondControlBezierPoint = firstControlBezierPoint;
                    clickCounter = 4;
                    return;
                }

                if (line != null)
                    myImage.Children.Remove(line);
                if (myPoints != null)
                {
                    for (int i = 0; i < myPoints.Length - 1; i++)
                        myImage.Children.RemoveAt(0);
                }
                endBezierPoint = e.GetPosition(myImage);
                secondControlBezierPoint = firstControlBezierPoint;
                DrawMyBezierCurve();
            }
            else if(currentMode == "bezierline" && clickCounter == 4)
            {
                if (line != null)
                    myImage.Children.Remove(line);
                if (myPoints != null)
                {
                    for (int i = 0; i < myPoints.Length - 1; i++)
                        myImage.Children.RemoveAt(0);
                }
                secondControlBezierPoint = e.GetPosition(myImage);
                DrawMyBezierCurve();
            }
        }

        private void myImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                else
                {
                    myStatusText.Text = "Za szybko !!! Odczekaj dwie sekundy i podaj punkt początkowy jeszcze raz.";
                    clickCounter = 0;
                }
            }
            else if (currentMode == "handline")
                startPoint = e.GetPosition(myImage);
            else if (currentMode == "bezierline" && (clickCounter == 0 || clickCounter == 4)) 
            {
                clickCounter++;
                if (clickCounter == 1)
                {
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj pierwszy punkt kontrolny.";
                    startBezierPoint = e.GetPosition(myImage);
                    startPoint = startBezierPoint;
                }
                if (clickCounter == 5)
                {
                    myStatusText.Text = "Wybrano tryb rysowania krzywych Beziera. Podaj punkt początkowy.";
                    clickCounter = 0;
                    myPoints = null;
                    line = null;
                }
            }
        }
       
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            Button mybtn = (Button)sender;
            clickCounter = 0;
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