using MySql.Data.MySqlClient;
using pra.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace pra.Services
{
    public class AlumnoService
    {
        private readonly string connectionString = "server=localhost;user=root;password=;database=alumnosdb";

        // Obtener todos los alumnos
        public async Task<ObservableCollection<Alumno>> GetAlumnosAsync()
        {
            var alumnos = new ObservableCollection<Alumno>();
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT * FROM alumnos", conn);
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    alumnos.Add(new Alumno
                    {
                        Matricula = reader.GetString("matricula"),
                        Nombre = reader.GetString("nombre"),
                        Apellidos = reader.GetString("apellidos"),
                        Correo = reader.GetString("correo"),
                        Carrera = reader.GetString("carrera"),
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error de conexión: {ex.Message}", "OK");
            }

            return alumnos;
        }

        // Agregar un alumno
        public async Task AddAlumnoAsync(Alumno alumno)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();

                var cmd = new MySqlCommand("INSERT INTO alumnos (matricula, nombre, apellidos, correo, carrera) VALUES (@matricula, @nombre, @apellidos, @correo, @carrera)", conn);
                cmd.Parameters.AddWithValue("@matricula", alumno.Matricula);
                cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
                cmd.Parameters.AddWithValue("@apellidos", alumno.Apellidos);
                cmd.Parameters.AddWithValue("@correo", alumno.Correo);
                cmd.Parameters.AddWithValue("@carrera", alumno.Carrera);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al agregar: {ex.Message}", "OK");
            }
        }

        // Actualizar alumno
        public async Task UpdateAlumnoAsync(Alumno alumno)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();

                var cmd = new MySqlCommand("UPDATE alumnos SET nombre=@nombre, apellidos=@apellidos, correo=@correo, carrera=@carrera WHERE matricula=@matricula", conn);
                cmd.Parameters.AddWithValue("@matricula", alumno.Matricula);
                cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
                cmd.Parameters.AddWithValue("@apellidos", alumno.Apellidos);
                cmd.Parameters.AddWithValue("@correo", alumno.Correo);
                cmd.Parameters.AddWithValue("@carrera", alumno.Carrera);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al actualizar: {ex.Message}", "OK");
            }
        }

        // Eliminar alumno
        public async Task DeleteAlumnoAsync(string matricula)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();

                var cmd = new MySqlCommand("DELETE FROM alumnos WHERE matricula=@matricula", conn);
                cmd.Parameters.AddWithValue("@matricula", matricula);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
            }
        }
        public async Task<bool> ProbarConexionAsync()
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
