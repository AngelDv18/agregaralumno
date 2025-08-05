using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;
using pra.Models;
using pra.Services;

namespace pra.ViewModels
{
    public class AlumnoViewModel : INotifyPropertyChanged
    {
        private readonly AlumnoService _service = new(); // Puedes usar inyección si prefieres

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Alumno alumno)
            {
                // Ya se asigna a AlumnoActual automáticamente gracias al Binding
                // Puedes forzar navegación o visualización si deseas
            }
        }

        public ObservableCollection<Alumno> ListaAlumnos { get; set; } = new();

        private Alumno alumnoActual = new();
        public Alumno AlumnoActual
        {
            get => alumnoActual;
            set
            {
                alumnoActual = value;
                OnPropertyChanged();
            }
        }

        public ICommand AgregarCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }

        public AlumnoViewModel()
        {
            AgregarCommand = new Command(async () => await AgregarAlumno());
            EditarCommand = new Command(async () => await EditarAlumno());
            EliminarCommand = new Command(async () => await EliminarAlumno());
            _ = CargarAlumnos();
        }

        private async Task CargarAlumnos()
        {
            bool conectado = await _service.ProbarConexionAsync();
            if (!conectado)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo conectar a la base de datos.", "OK");
                return;
            }
            try
            {
                var lista = await _service.GetAlumnosAsync();
                ListaAlumnos.Clear();
                foreach (var a in lista)
                    ListaAlumnos.Add(a);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error al cargar alumnos.\n{ex.Message}", "OK");
            }
        }

        private async Task AgregarAlumno()
        {
            if (!ValidarEntrada(out var mensaje))
            {
                await App.Current.MainPage.DisplayAlert("Validación", mensaje, "OK");
                return;
            }

            try
            {
                await _service.AddAlumnoAsync(AlumnoActual);
                await App.Current.MainPage.DisplayAlert("Éxito", "Alumno agregado.", "OK");
                AlumnoActual = new Alumno();
                await CargarAlumnos();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error al agregar.\n{ex.Message}", "OK");
            }
        }

        private async Task EditarAlumno()
        {
            if (!ValidarEntrada(out var mensaje))
            {
                await App.Current.MainPage.DisplayAlert("Validación", mensaje, "OK");
                return;
            }

            try
            {
                await _service.UpdateAlumnoAsync(AlumnoActual);
                await App.Current.MainPage.DisplayAlert("Éxito", "Alumno actualizado.", "OK");
                AlumnoActual = new Alumno();
                await CargarAlumnos();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error al editar.\n{ex.Message}", "OK");
            }
        }

        private async Task EliminarAlumno()
        {
            if (string.IsNullOrWhiteSpace(AlumnoActual?.Matricula))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Selecciona un alumno primero.", "OK");
                return;
            }

            var confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Eliminar alumno?", "Sí", "No");
            if (!confirm) return;

            try
            {
                await _service.DeleteAlumnoAsync(AlumnoActual.Matricula);
                await App.Current.MainPage.DisplayAlert("Éxito", "Alumno eliminado.", "OK");
                AlumnoActual = new Alumno();
                await CargarAlumnos();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error al eliminar.\n{ex.Message}", "OK");
            }
        }

        private bool ValidarEntrada(out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(AlumnoActual.Matricula) ||
                string.IsNullOrWhiteSpace(AlumnoActual.Nombre) ||
                string.IsNullOrWhiteSpace(AlumnoActual.Apellidos) ||
                string.IsNullOrWhiteSpace(AlumnoActual.Correo) ||
                string.IsNullOrWhiteSpace(AlumnoActual.Carrera))
            {
                mensaje = "Todos los campos son obligatorios.";
                return false;
            }

            if (!Regex.IsMatch(AlumnoActual.Nombre, @"^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]+$") ||
                !Regex.IsMatch(AlumnoActual.Apellidos, @"^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]+$"))
            {
                mensaje = "Nombre y Apellidos solo deben contener letras.";
                return false;
            }

            if (!Regex.IsMatch(AlumnoActual.Correo, @"^[\w\.-]+@(gmail\.com|hotmail\.com|outlook\.com|upqroo\.edu\.mx)$"))
            {
                mensaje = "Correo no válido.";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }


}
