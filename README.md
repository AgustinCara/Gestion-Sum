# 🏢 GestionSUM - Sistema de Reservas para Edificios

![Status](https://img.shields.io/badge/Status-Finalizado-green)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![MySQL](https://img.shields.io/badge/MySQL-8.0-orange)

**GestionSUM** es una plataforma web Full Stack diseñada para simplificar la reserva de espacios comunes (SUM, parrilla, salón de eventos) en complejos residenciales. Elimina la necesidad de planillas manuales o coordinación vía chat, centralizando todo en un calendario interactivo.

---

## 📸 Capturas de Pantalla

| Inicio  | Calendario Interactivo |
| :---: | :---: |
| ![Inicio](Screenshots/Home.png) | ![Calendario](Screenshots/Calendario.png) |

| Nueva Reserva | Registro de Usuarios |
| :---: | :---: |
| ![Reserva](Screenshots/NuevaReserva.png) | ![Registro](Screenshots/Registro.png) |

---

## 🚀 Características Principales

- **Gestión de Roles Dinámica:** Diferenciación clara entre **Administradores** y **Vecinos** mediante ASP.NET Core Identity.
- **Calendario Interactivo:** Integración con **FullCalendar API** para visualizar la disponibilidad en tiempo real.
- **Reglas de Negocio Automatizadas:** - Anticipación mínima de 6 horas para reservar.
  - Bloqueo de fechas pasadas y validación de solapamientos de turnos.
  - Cancelación permitida hasta 2 horas antes (solo para vecinos).
- **Notificaciones por Email:** Envío automático de comprobantes de reserva mediante **MailKit**.
- **Internacionalización:** Mensajes de error y validaciones totalmente traducidos al español.

---

## 🛠️ Tecnologías Utilizadas

- **Backend:** C# / ASP.NET Core MVC 8.
- **ORM:** Entity Framework Core.
- **Base de Datos:** MySQL / MariaDB.
- **Frontend:** HTML5, CSS3 (Glassmorphism design), Bootstrap 5, JavaScript.
- **Librerías:** FullCalendar API, MailKit, FontAwesome / Bootstrap Icons.

---

## 🔧 Configuración Local

1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/tu-usuario/GestionSUM.git](https://github.com/tu-usuario/GestionSUM.git)


2. **Configurar la Base de Datos:**
    ```bash
   "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=sum_reservas;user=root;password=TU_PASSWORD;"
}

3. **Ejecutar Migraciones:**
    ```bash
    Update-Database

4. **Ejecutar la Aplicación:**
    ```bash
    dotnet run


👤 Autor
Agustín Carabajal Systems Professional & Full Stack Developer
agustin.hcarabajal@gmail.com