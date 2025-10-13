Lo primero que se hizo fue crear la tabla de datos para poder hacer un scaffold y llevar un contexto legible a el framework de trabajo EF. De esta manera, se ejecutaron las siguientes líneas SQL para crear cada tabla, sus llaves primararias, y las respectivas relaciones de acuerdo al MER.

```SQL
USE [PruebaTecnica1];
GO

CREATE TABLE dbo.editoriales
(
    id     INT         IDENTITY(1,1) NOT NULL,
    nombre VARCHAR(50) NOT NULL,
    sede   VARCHAR(50) NOT NULL,

    CONSTRAINT PK_editoriales PRIMARY KEY CLUSTERED (id)
);
GO

CREATE TABLE dbo.autores
(
    id        INT         IDENTITY(1,1) NOT NULL,
    nombre    VARCHAR(50) NOT NULL,
    apellidos VARCHAR(50) NOT NULL,

    CONSTRAINT PK_autores PRIMARY KEY CLUSTERED (id)
);
GO

CREATE TABLE dbo.libros
(
    ISBN           INT         NOT NULL,
    editoriales_id INT         NOT NULL,
    titulo         VARCHAR(50) NOT NULL,
    sipnosis       TEXT        NOT NULL,   -- o VARCHAR(MAX)
    n_paginas      VARCHAR(50) NOT NULL,

    CONSTRAINT PK_libros PRIMARY KEY CLUSTERED (ISBN),

    -- FK inline: la tabla referenciada (editoriales) ya existe
    CONSTRAINT FK_libros_editoriales
        FOREIGN KEY (editoriales_id)
        REFERENCES dbo.editoriales(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX IX_libros_editoriales_id
    ON dbo.libros (editoriales_id);
GO

/* 3) Tabla puente (FKs inline a autores y libros) */
CREATE TABLE dbo.autores_has_libros
(
    autores_id  INT NOT NULL,
    libros_ISBN INT NOT NULL,

    CONSTRAINT PK_autores_has_libros
        PRIMARY KEY CLUSTERED (autores_id, libros_ISBN),

    CONSTRAINT FK_ahl_autores
        FOREIGN KEY (autores_id)
        REFERENCES dbo.autores(id)
        ON DELETE CASCADE
        ON UPDATE NO ACTION,

    CONSTRAINT FK_ahl_libros
        FOREIGN KEY (libros_ISBN)
        REFERENCES dbo.libros(ISBN)
        ON DELETE CASCADE
        ON UPDATE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX IX_ahl_libros_ISBN
    ON dbo.autores_has_libros (libros_ISBN, autores_id);
GO

```

Hace falta mencionar la creación de los índices "No Clustered" para agilizar la ejecución de consultas en la base de datos. Una vez creadas las tablas, se asignaran valores a la tablas para futuras pruebas de los servicios creados en los controladores. En este sentido, se uso el siguiente comando SQL.

```sql
BEGIN TRAN;


INSERT INTO dbo.editoriales (id, nombre, sede) VALUES
(1,  'Albatros',            'Bogotá'),
(2,  'Monteverde',          'Medellín'),
(3,  'Estrella del Sur',    'Cali'),
(4,  'Norte Editorial',     'Barranquilla'),
(5,  'Río Grande',          'Cartagena'),
(6,  'Andes Press',         'Bucaramanga'),
(7,  'Pacífico',            'Pereira'),
(8,  'Horizonte',           'Manizales'),
(9,  'Luna Nueva',          'Santa Marta'),
(10, 'Travesía',            'Villavicencio');




INSERT INTO dbo.autores (id, nombre, apellidos) VALUES
(1,  'Ana',      'Ramírez'),
(2,  'Luis',     'González'),
(3,  'María',    'Pérez'),
(4,  'Jorge',    'Rodríguez'),
(5,  'Paula',    'Martínez'),
(6,  'Camilo',   'Hernández'),
(7,  'Sofía',    'López'),
(8,  'Andrés',   'García'),
(9,  'Laura',    'Torres'),
(10, 'Felipe',   'Castro'),
(11, 'Valeria',  'Vargas'),
(12, 'Diego',    'Suárez'),
(13, 'Carolina', 'Romero'),
(14, 'Daniel',   'Ortiz'),
(15, 'Isabella', 'Cortés'),
(16, 'Sebastián','Moreno'),
(17, 'Juliana',  'Ríos'),
(18, 'Mateo',    'Patiño'),
(19, 'Manuela',  'Cárdenas'),
(20, 'Nicolás',  'Mendoza'),
(21, 'Gabriela', 'Salazar'),
(22, 'Tomás',    'Reyes'),
(23, 'Sara',     'Pineda'),
(24, 'Simón',    'Guzmán'),
(25, 'Elena',    'Navarro');



INSERT INTO dbo.libros (ISBN, editoriales_id, titulo, sipnosis, n_paginas) VALUES
(1001,  1,  'Introducción a C#',                        'Fundamentos del lenguaje C# y .NET.',                           '240'),
(1002,  2,  'Patrones de Diseño Aplicados',            'Uso práctico de patrones GoF en proyectos reales.',             '320'),
(1003,  3,  'ASP.NET Core para APIs',                  'Diseño y construcción de APIs REST con ASP.NET Core.',          '280'),
(1004,  4,  'Entity Framework Core en Acción',         'Mapeo, consultas y rendimiento con EF Core.',                   '350'),
(1005,  5,  'Pruebas con NUnit',                       'Estrategias de testing unitario y de integración.',             '210'),
(1006,  6,  'SQL Server Esencial',                     'Consultas, índices y optimización en SQL Server.',              '400'),
(1007,  7,  'LINQ Profundo',                           'Consultas expresivas y proyección de datos.',                   '190'),
(1008,  8,  'Clean Architecture en .NET',              'Capas, dependencias y casos de uso limpios.',                   '330'),
(1009,  9,  'DDD Hecho Práctico',                      'Dominio, agregados y repositorios en proyectos reales.',        '360'),
(1010, 10,  'Microservicios en .NET',                  'Diseño, comunicación y despliegue de microservicios.',          '300'),
(1011,  1,  'Seguridad en APIs',                       'Autenticación, autorización y mejores prácticas.',              '270'),
(1012,  2,  'Docker para Desarrolladores .NET',        'Contenerización y flujos de trabajo locales.',                  '230'),
(1013,  3,  'Kubernetes Básico',                       'Conceptos y despliegue de servicios.',                          '260'),
(1014,  4,  'Mensajería y Event-Driven',               'RabbitMQ, colas y pub/sub en .NET.',                            '245'),
(1015,  5,  'Performance Tuning en .NET',              'Profiling, memoria y throughput.',                               '310'),
(1016,  6,  'CQRS y Mediator',                         'Separación de comandos/consultas con patrones modernos.',       '220'),
(1017,  7,  'Practicas DevOps para .NET',              'CI/CD, entornos y automatización.',                             '275'),
(1018,  8,  'Integración con Terceros',                'OAuth, Webhooks y SDKs.',                                       '200'),
(1019,  9,  'Observabilidad',                          'Logs, métricas y trazas distribuidas.',                         '260'),
(1020, 10,  'Refactorización Segura',                  'Técnicas para mejorar código sin romper producción.',           '240'),
(1021,  1,  'Fundamentos de Redes',                    'TCP/IP, HTTP y diagnósticos.',                                  '210'),
(1022,  2,  'Criptografía Aplicada',                   'Hashing, cifrado y firmas en sistemas.',                        '190'),
(1023,  3,  'Sistemas Distribuidos',                   'Consistencia, particiones y tolerancia a fallos.',              '360'),
(1024,  4,  'Algoritmos y Estructuras de Datos',       'Listas, grafos y complejidad.',                                 '410'),
(1025,  5,  'UI/UX para Backend Devs',                 'Diseño centrado en el usuario para APIs y tooling.',            '180');


INSERT INTO dbo.autores_has_libros (autores_id, libros_ISBN) VALUES
-- Libro 1001
(1, 1001), (2, 1001),
-- 1002
(3, 1002), (4, 1002),
-- 1003
(5, 1003), (6, 1003), (7, 1003),
-- 1004
(8, 1004), (9, 1004),
-- 1005
(10, 1005), (11, 1005),
-- 1006
(12, 1006), (13, 1006), (14, 1006),
-- 1007
(15, 1007), (16, 1007),
-- 1008
(17, 1008), (18, 1008), (19, 1008),
-- 1009
(20, 1009), (21, 1009),
-- 1010
(22, 1010), (23, 1010), (24, 1010),
-- 1011
(25, 1011), (1, 1011),
-- 1012
(2, 1012), (3, 1012),
-- 1013
(4, 1013), (5, 1013), (6, 1013),
-- 1014
(7, 1014), (8, 1014),
-- 1015
(9, 1015), (10, 1015), (11, 1015),
-- 1016
(12, 1016), (13, 1016),
-- 1017
(14, 1017), (15, 1017), (16, 1017),
-- 1018
(17, 1018), (18, 1018),
-- 1019
(19, 1019), (20, 1019), (21, 1019),
-- 1020
(22, 1020), (23, 1020),
-- 1021
(24, 1021), (25, 1021),
-- 1022
(1, 1022), (2, 1022), (3, 1022),
-- 1023
(4, 1023), (5, 1023),
-- 1024
(6, 1024), (7, 1024), (8, 1024),
-- 1025
(9, 1025), (10, 1025);

COMMIT;

```