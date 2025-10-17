
Lo primero que se hace es instanciar la base de la URL para las futuras consultas. Luego, se crean 2 funciones base:

- **buildQuery**: Encargada de recibir los parámetros que el cliente puede ingresar, validarlos y terminar de construir la URL para llamar el servicio en el Back. 

- **getJsonOrThrow**: Encargada de hacer el llamado al servicio dependiendo de la URL entregada, maneja errores y aborta la solicitud si hay un exceso de tiempo en la ejecución.

Estas dos funciones son bases dado que son llamadas en las funciones especificas **fetchLibros** y **fetchLibrosPorIsbn** para así construir la URL y posteriormente llamar el servicio en el Back.
