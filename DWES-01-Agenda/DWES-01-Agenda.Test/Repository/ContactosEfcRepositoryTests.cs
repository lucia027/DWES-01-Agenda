using DWES;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Agenda.Test.Repository;

[TestFixture]
public class ContactosEfcRepositoryTests {

    [TestFixture]
    public sealed class CasosValidos {

        private SqliteConnection _connection = null!;
        private AppDbContext _context = null!;
        private ContactosEfcRepository _repository = null!;

        [SetUp]
        public void SetUp() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new ContactosEfcRepository(_context, dropData: true, seedData: false);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public void Constructor_DropDataFalse_MantieneDatos() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            });

            //Act
            var repository = new ContactosEfcRepository(
                _context,
                dropData: false,
                seedData: false
            );

            var res = repository.GetAll();

            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(1);
        }

        [Test]
        public void Constructor_SeedDataTrue_CreaContactos() {
            //Act
            var repository = new ContactosEfcRepository(
                _context,
                dropData: true,
                seedData: true
            );

            var res = repository.GetAll(1, 100);

            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(50);
        }

        [Test]
        public void GetAll_ContactosExistentes_RetornaContactos() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            });

            _repository.Create(new Contacto {
                Id = 2,
                Alias = "Noa",
                Nombre = "Noah Diaz",
                Telefono = "777777777",
                Email = "noah@gmail.com"
            });

            //Act
            var res = _repository.GetAll();

            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(2);
        }

        [Test]
        public void GetAll_VariosContactos_RetornaOrdenadosPorNombre() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Zoe",
                Nombre = "Zoe Martinez",
                Telefono = "611111111",
                Email = "zoe@gmail.com"
            });

            _repository.Create(new Contacto {
                Id = 2,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "622222222",
                Email = "laura@gmail.com"
            });

            _repository.Create(new Contacto {
                Id = 3,
                Alias = "Ana",
                Nombre = "Ana Lopez",
                Telefono = "633333333",
                Email = "ana@gmail.com"
            });

            //Act
            var res = _repository.GetAll().ToList();

            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(3);

            res[0].Nombre.Should().Be("Ana Lopez");
            res[1].Nombre.Should().Be("Laura Ruiz");
            res[2].Nombre.Should().Be("Zoe Martinez");
        }

        [Test]
        public void GetAll_Paginacion_RetornaPaginaSolicitada() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Ana",
                Nombre = "Ana Lopez",
                Telefono = "611111111",
                Email = "ana@gmail.com"
            });

            _repository.Create(new Contacto {
                Id = 2,
                Alias = "Laura",
                Nombre = "Laura Ruiz",
                Telefono = "622222222",
                Email = "laura@gmail.com"
            });

            _repository.Create(new Contacto {
                Id = 3,
                Alias = "Noa",
                Nombre = "Noah Diaz",
                Telefono = "633333333",
                Email = "noah@gmail.com"
            });

            //Act
            var res = _repository.GetAll(2, 2).ToList();

            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(1);
            res[0].Nombre.Should().Be("Noah Diaz");
        }

        [Test]
        public void GetById_ContactoExistente_RetornaContacto() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            _repository.Create(contacto);

            //Act
            var res = _repository.GetById(1);

            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();

            res.Value.Id.Should().Be(1);
            res.Value.Alias.Should().Be("Laurita");
            res.Value.Nombre.Should().Be("Laura Ruiz");
            res.Value.Telefono.Should().Be("666666666");
            res.Value.Email.Should().Be("laura@gmail.com");
        }

        [Test]
        public void GetByAlias_ContactoExistente_RetornaContacto() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            });

            //Act
            var res = _repository.GetByAlias("Laurita");

            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();

            res.Value.Alias.Should().Be("Laurita");
            res.Value.Nombre.Should().Be("Laura Ruiz");
        }

        [Test]
        public void Create_ContactoValido_CreaContacto() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com",
                CreateAt = new DateTime(2026, 04, 16),
                UpdateAt = new DateTime(2026, 04, 20),
                DeleteAt = new DateTime(2026, 04, 21),
                IsDelete = true
            };

            var inicio = DateTime.Now;

            //Act
            var res = _repository.Create(contacto);

            var fin = DateTime.Now;

            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();

            res.Value.Alias.Should().Be("Laurita");
            res.Value.Nombre.Should().Be("Laura Ruiz");
            res.Value.Telefono.Should().Be("666666666");
            res.Value.Email.Should().Be("laura@gmail.com");

            res.Value.CreateAt.Should().BeOnOrAfter(inicio);
            res.Value.CreateAt.Should().BeOnOrBefore(fin);

            res.Value.UpdateAt.Should().BeNull();
            res.Value.DeleteAt.Should().BeNull();
            res.Value.IsDelete.Should().BeFalse();
        }

        [Test]
        public void Update_ContactoValido_ActualizaContacto() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            });

            var contactoNuevo = new Contacto {
                Id = 1,
                Alias = "Laura",
                Nombre = "Laura Ruiz Garcia",
                Telefono = "699999999",
                Email = "laura.ruiz@gmail.com",
                DeleteAt = DateTime.Now,
                IsDelete = true
            };

            var inicio = DateTime.Now;

            //Act
            var res = _repository.Update(1, contactoNuevo);

            var fin = DateTime.Now;

            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();

            res.Value.Id.Should().Be(1);
            res.Value.Alias.Should().Be("Laura");
            res.Value.Nombre.Should().Be("Laura Ruiz Garcia");
            res.Value.Telefono.Should().Be("699999999");
            res.Value.Email.Should().Be("laura.ruiz@gmail.com");

            res.Value.UpdateAt.Should().NotBeNull();
            res.Value.UpdateAt!.Value.Should().BeOnOrAfter(inicio);
            res.Value.UpdateAt!.Value.Should().BeOnOrBefore(fin);

            res.Value.DeleteAt.Should().BeNull();
            res.Value.IsDelete.Should().BeFalse();
        }

        [Test]
        public void Delete_ContactoExistente_MarcaContactoEliminado() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            });

            var inicio = DateTime.Now;

            //Act
            var res = _repository.Delete(1);

            var fin = DateTime.Now;

            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();

            res.Value.IsDelete.Should().BeTrue();
            res.Value.DeleteAt.Should().NotBeNull();

            res.Value.DeleteAt!.Value.Should().BeOnOrAfter(inicio);
            res.Value.DeleteAt!.Value.Should().BeOnOrBefore(fin);
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        private SqliteConnection _connection = null!;
        private AppDbContext _context = null!;
        private ContactosEfcRepository _repository = null!;

        [SetUp]
        public void SetUp() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new ContactosEfcRepository(
                _context,
                dropData: true,
                seedData: false
            );
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();

            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public void GetAll_AlmacenVacio_RetornaVacio() {
            //Act
            var res = _repository.GetAll();

            //Assert
            res.Should().NotBeNull();
            res.Should().BeEmpty();
        }

        [Test]
        public void GetById_ContactoInexistente_RetornaFallo() {
            //Act
            var res = _repository.GetById(66);

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void GetByAlias_ContactoInexistente_RetornaFallo() {
            //Act
            var res = _repository.GetByAlias("NoExiste");

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void GetByAlias_AliasVacio_RetornaFallo() {
            //Act
            var res = _repository.GetByAlias();

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Create_ContactoNulo_RetornaFallo() {
            //Arrange
            Contacto contacto = null!;

            //Act
            var res = _repository.Create(contacto);

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Update_ContactoInexistente_RetornaFallo() {
            //Arrange
            var contacto = new Contacto {
                Id = 66,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            //Act
            var res = _repository.Update(66, contacto);

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Update_ContactoNulo_RetornaFallo() {
            //Arrange
            _repository.Create(new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            });

            Contacto contacto = null!;

            //Act
            var res = _repository.Update(1, contacto);

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Delete_ContactoInexistente_RetornaFallo() {
            //Act
            var res = _repository.Delete(66);

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Delete_ContextoNoDisponible_RetornaFallo() {
            //Arrange
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();

            var repository = new ContactosEfcRepository(
                context,
                dropData: false,
                seedData: false
            );

            context.Dispose();

            //Act
            var res = repository.Delete(1);

            //Assert
            res.Should().NotBeNull();
            res.IsFailure.Should().BeTrue();

            connection.Dispose();
        }
    }
}