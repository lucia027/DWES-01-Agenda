using DWES;
using FluentAssertions;
using NUnit.Framework;

namespace Agenda.Test.Mapper;

[TestFixture]
public class ContactoMapperTests {

    [TestFixture]
    public sealed class CasosValidos {

        [SetUp]
        public void Setup() {
            _contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com",
                CreateAt = new DateTime(2026, 04, 16),
                UpdateAt = new DateTime(2026, 04, 20),
                DeleteAt = null,
                IsDelete = false
            };

            _contactoEntity = new ContactoEntity {
                Id = 2,
                Alias = "Noa",
                Nombre = "Noah Diaz",
                Telefono = "77777777",
                Email = "noah@gmail.com",
                CreateAt = new DateTime(2026, 04, 16),
                UpdateAt = new DateTime(2026, 04, 20),
                DeleteAt = null,
                IsDelete = false
            };
        }

        private Contacto _contacto = null!;
        private ContactoEntity _contactoEntity = null!;

        [Test]
        public void ToModel_ContactoEntity_ConvierteCorrectamente() {
            //Act
            var res = _contactoEntity.ToModel();

            //Assert
            res.Should().NotBeNull();
            res.Id.Should().Be(2);
            res.Alias.Should().Be("Noa");
            res.Nombre.Should().Be("Noah Diaz");
            res.Telefono.Should().Be("77777777");
            res.Email.Should().Be("noah@gmail.com");
            res.CreateAt.Should().Be(new DateTime(2026, 04, 16));
            res.UpdateAt.Should().Be(new DateTime(2026, 04, 20));
            res.DeleteAt.Should().BeNull();
            res.IsDelete.Should().BeFalse();
        }

        [Test]
        public void ToEntity_Contacto_ConvierteCorrectamente() {
            //Act
            var res = _contacto.ToEntity();

            //Assert
            res.Should().NotBeNull();
            res.Id.Should().Be(1);
            res.Alias.Should().Be("Laurita");
            res.Nombre.Should().Be("Laura Ruiz");
            res.Telefono.Should().Be("666666666");
            res.Email.Should().Be("laura@gmail.com");
            res.CreateAt.Should().Be(new DateTime(2026, 04, 16));
            res.UpdateAt.Should().Be(new DateTime(2026, 04, 20));
            res.DeleteAt.Should().BeNull();
            res.IsDelete.Should().BeFalse();
        }

        [Test]
        public void ToModel_ColeccionContactoEntity_ConvierteCorrectamente() {
            //Arrange
            var contactos = new List<ContactoEntity> { _contactoEntity, _contacto.ToEntity() };

            //Act
            var res = contactos.ToModel().ToList();

            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(2);

            res[0].Id.Should().Be(2);
            res[0].Alias.Should().Be("Noa");
            res[0].Nombre.Should().Be("Noah Diaz");
            res[0].Telefono.Should().Be("77777777");
            res[0].Email.Should().Be("noah@gmail.com");
            res[0].CreateAt.Should().Be(new DateTime(2026, 04, 16));
            res[0].UpdateAt.Should().Be(new DateTime(2026, 04, 20));
            res[0].DeleteAt.Should().BeNull();
            res[0].IsDelete.Should().BeFalse();

            res[1].Id.Should().Be(1);
            res[1].Alias.Should().Be("Laurita");
            res[1].Nombre.Should().Be("Laura Ruiz");
            res[1].Telefono.Should().Be("666666666");
            res[1].Email.Should().Be("laura@gmail.com");
            res[1].CreateAt.Should().Be(new DateTime(2026, 04, 16));
            res[1].UpdateAt.Should().Be(new DateTime(2026, 04, 20));
            res[1].DeleteAt.Should().BeNull();
            res[1].IsDelete.Should().BeFalse();
        }

        [Test]
        public void ToModel_ColeccionVacia_RetornaColeccionVacia() {
            //Arrange
            var contactos = new List<ContactoEntity>();

            //Act
            var res = contactos.ToModel();

            //Assert
            res.Should().NotBeNull();
            res.Should().BeEmpty();
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {

        [Test]
        public void ToModel_ContactoEntityNulo_LanzaExcepcion() {
            //Arrange
            ContactoEntity contactoEntity = null!;

            //Act
            var accion = () => contactoEntity.ToModel();

            //Assert
            accion.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void ToEntity_ContactoNulo_LanzaExcepcion() {
            //Arrange
            Contacto contacto = null!;

            //Act
            var accion = () => contacto.ToEntity();

            //Assert
            accion.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void ToModel_ColeccionNula_LanzaExcepcion() {
            //Arrange
            IEnumerable<ContactoEntity> contactos = null!;

            //Act
            var accion = () => contactos.ToModel();

            //Assert
            accion.Should().Throw<ArgumentNullException>();
        }
    }
}