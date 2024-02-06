using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NanoPSI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static MainWindow()
        {
            // Set Deployment Key for LightningChart components
            string deploymentKey = "lgCAAE3C1L0Ig9kBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTA1LTEyI1JldmlzaW9uPTACgD9VCrEQWTcyiWKli80/h/yvJ3PUo9Iacz9kMGmQRMVUlMQFidiOQwCSaOXmsMNu7XFpqENfqtDrAAoepDhhY1eAnA1aWsJYAdPY01icT/20IDzzsnhuXuyWwRO9w4lCbieMD4do7M6U1Wdj0+xAPRuZGrSY3kaJYnp84YvfZ1wPrvvhXbt/EQPkcKstrpo7kjxhEjSEL3p+rIQfz1zde1WFLR0VmMbTZe4M3c7Z5qS9aPQtPT3GDNeQFgtEBdUhFNjh1xVJguD6y8rjVmqyzkelfnNyj/6zMh/C5rZSaBWpmmSNIgD+nJ3CRyA3DoYXDVTlxmbLP16UV0YvttIC/5SJD15lQkPbd+B/KJYc8Ost0wBYmHGR9/lveXThh/NWz8amQyE3It/nn2S7bvWmJoGOtsbqRCsyeGbZBxW2EnNQnlruvy0tlzQ8FvaMlXCeB4LYQntdi12EKGh8QYIxuEdu5BBquigN3fChsf+r+qjE9+hIXXMIgi5YfhTJU9meik8=";

            // Setting Deployment Key for non-bindable chart
            Arction.Wpf.Charting.LightningChart.SetDeploymentKey(deploymentKey);
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
