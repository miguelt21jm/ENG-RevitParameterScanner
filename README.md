# ENG-RevitParameterScanner
ENG Challenge: Revit Parameter Scanner
![image](https://github.com/miguelt21jm/ENG-RevitParameterScanner/assets/101025223/9d82a730-6843-49d2-a2ec-77107a222169)

## Revit Parameter scanner
# Requierments
Windows 10/11, .NET Framework 4.8.

# Setup
We make use of PostBuild events for copying all the dependencies, so you will be done by download this repository and building the solution in order to have all the necessary dependencies on you computer's revit installation.

# Use
Click the "Parameter Scanner" button, and the interface will guide you through the process. 

The main idea behind this design is that you should scan the parameters of a view, if you want to select or isolate elements. This approach is intentional because querying the parameters of all elements each time you want to select or isolate one could degrade the user experience, particularly in large models where extensive use is anticipated.

## Functionality overview
This tool can only scan Floor Plans, Reflected Ceiling Plans, and 3D Views:
![image](https://github.com/miguelt21jm/ENG-RevitParameterScanner/assets/101025223/6673859d-0caa-4a49-9eb1-ad01cb658981)


Once a view is scanned, you will be able to select parameters identified during the scan.
![image](https://github.com/miguelt21jm/ENG-RevitParameterScanner/assets/101025223/26a77f57-4cfc-46f7-9960-1a80798fc437)

* After a view has been successfully scanned, you can navigate between multiple views without needing to perform additional scans.


Once a valid parameter is selected, you will be able to see its values.*This works even with empty values:
![image](https://github.com/miguelt21jm/ENG-RevitParameterScanner/assets/101025223/363eb634-2126-4fc5-942c-b1e74cae76dc)

If an element within the view is deleted after a scan, attempting to select or isolate this element will trigger a warning message advising you to rescan the view.
![image](https://github.com/miguelt21jm/ENG-RevitParameterScanner/assets/101025223/e286525d-fc94-442e-a19b-734b8a4d4dac)

# Technical note
We utilize the WPF UI by Lepoco, for our user interface elements.

We employ Microsoft.Extensions.DependencyInjection to utilize the IoC container (Service Provider).

To improve user interface responsiveness and simplicity, we incorporate CalcBinding.

For addressing compatibility issues, we use PolySharp.

To manage MVVM boilerplate code, we utilize CommunityToolkit.Mvvm.
