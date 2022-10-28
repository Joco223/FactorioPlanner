using Microsoft.Win32;
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
using System.Diagnostics;

namespace FactorioPlanner {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        Project mainProject = new Project();

        public MainWindow() {
            InitializeComponent();
        }

        private void newProjectButton_Click(object sender, RoutedEventArgs e) {
            mainProject = new Project("Untitled");
        }

        private void saveProject() {
            mainProject.NewProject = false;

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = mainProject.Name;
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON file (*.json) |*.json";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() == true) {
                mainProject.Path = dialog.FileName;
                mainProject.Name = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                mainProject.saveProject(dialog.FileName);       
            }
        }

        private void saveProjectButton_Click(object sender, RoutedEventArgs e) {
            if (mainProject.NewProject) {
                saveProject();
            } else {
                mainProject.NewProject = false;
                mainProject.saveProject(mainProject.Path);
            }
        }

        private void saveAsProjectButton_Click(object sender, RoutedEventArgs e) {
            saveProject();
        }

        private void openProjectButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true) {
                mainProject.loadProject(dialog.FileName);
            }
        }

        private void loadDefaultRecipes_Click(object sender, RoutedEventArgs e) {
            mainProject.loadDefaultRecipes();
        }
    }
}
