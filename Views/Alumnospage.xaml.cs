using pra.ViewModels; // Asegúrate de que este espacio de nombres es correcto para tu proyecto

namespace pra.Views;

public partial class Alumnospage : ContentPage
{
	public Alumnospage()
	{
		InitializeComponent();
        BindingContext = new AlumnoViewModel(); // Asegúrate de tener el ViewModel correcto
    }
}