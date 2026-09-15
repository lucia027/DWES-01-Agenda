using CSharpFunctionalExtensions;
using DWES;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Agenda.Test.Service;

[TestFixture]
public class ContactoServiceTests {

    [TestFixture]
    public class CasosValidos : ContactoServiceTests {

        private Mock<IContactoRepository> _repository = null!;
        private Mock<ICache<int, Contacto>> _cache = null!;
        private ContactoService _service = null!;

        [SetUp]
        public void SetUp() {
            _repository = new Mock<IContactoRepository>();
            _cache = new Mock<ICache<int, Contacto>>();
            _service = new ContactoService(_repository.Object, _cache.Object);
        }

        [Test]
        public void GetAll_ContactosExistentes_RetornaListado() {
            //Arrange
            var contacto1 = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com",
                CreateAt = DateTime.Today,
                UpdateAt = null,
                DeleteAt = null,
                IsDelete = false
            };

            var contacto2 = new Contacto {
                Id = 2,
                Alias = "Noa",
                Nombre = "Noah Diaz",
                Telefono = "777777777",
                Email = "noah@gmail.com",
                CreateAt = DateTime.Today,
                UpdateAt = null,
                DeleteAt = null,
                IsDelete = false
            };

            var contactos = new List<Contacto> { contacto1, contacto2 };
            _repository.Setup(r => r.GetAll(1, 5)).Returns(contactos);

            //Act
            var res = _service.GetAll();

            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(2);
            res.Should().Contain(c => c.Alias == "Laurita");
            res.Should().Contain(c => c.Alias == "Noa");

            //Verify
            _repository.Verify(r => r.GetAll(1, 5), Times.Once);
        }

        [Test]
        public void GetById_ContactoEnCache_RetornaSuccess() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            _cache.Setup(c => c.Get(contacto.Id)).Returns(contacto);

            //Act
            var res = _service.GetById(contacto.Id);

            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEquivalentTo(contacto);

            //Verify
            _cache.Verify(c => c.Get(contacto.Id), Times.Once);
            _repository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetById_ContactoNoEstaEnCache_RetornaRepositorioYGuardaCache() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            _cache.Setup(c => c.Get(contacto.Id)).Returns((Contacto?)null);
            _repository.Setup(r => r.GetById(contacto.Id)).Returns(Result.Success<Contacto, DomainError>(contacto));

            //Act
            var res = _service.GetById(contacto.Id);

            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEquivalentTo(contacto);

            //Verify
            _cache.Verify(c => c.Get(contacto.Id), Times.Once);
            _repository.Verify(r => r.GetById(contacto.Id), Times.Once);
            _cache.Verify(c => c.Add(contacto.Id, contacto), Times.Once);
        }

        [Test]
        public void GetByAlias_ContactoExistente_RetornaSuccess() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            _repository.Setup(r => r.GetByAlias("Laurita")).Returns(Result.Success<Contacto, DomainError>(contacto));

            //Act
            var res = _service.GetByAlias("Laurita");

            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEquivalentTo(contacto);

            //Verify
            _repository.Verify(r => r.GetByAlias("Laurita"), Times.Once);
        }

        [Test]
        public void Create_ContactoValido_RetornaSuccess() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            _repository.Setup(r => r.Create(contacto)).Returns(Result.Success<Contacto, DomainError>(contacto));

            //Act
            var res = _service.Create(contacto);

            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEquivalentTo(contacto);

            //Verify
            _repository.Verify(r => r.Create(contacto), Times.Once);
            _cache.Verify(c => c.Add(contacto.Id, contacto), Times.Once);
        }

        [Test]
        public void Update_ContactoValido_RetornaSuccess() {
            //Arrange
            var contactoExistente = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            var contactoNuevo = new Contacto {
                Id = 1,
                Alias = "Laura",
                Nombre = "Laura Ruiz Garcia",
                Telefono = "699999999",
                Email = "laura.ruiz@gmail.com"
            };

            _repository.Setup(r => r.GetById(contactoExistente.Id)).Returns(Result.Success<Contacto, DomainError>(contactoExistente));
            _repository.Setup(r => r.Update(contactoExistente.Id, contactoNuevo)).Returns(Result.Success<Contacto, DomainError>(contactoNuevo));

            //Act
            var res = _service.Update(contactoExistente.Id, contactoNuevo);

            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Alias.Should().Be("Laura");
            res.Value.Nombre.Should().Be("Laura Ruiz Garcia");
            res.Value.Telefono.Should().Be("699999999");

            //Verify
            _repository.Verify(r => r.GetById(contactoExistente.Id), Times.Once);
            _repository.Verify(r => r.Update(contactoExistente.Id, contactoNuevo), Times.Once);
            _cache.Verify(c => c.Remove(contactoExistente.Id), Times.Once);
            _cache.Verify(c => c.Add(contactoExistente.Id, contactoNuevo), Times.Once);
        }

        [Test]
        public void Delete_ContactoValido_RetornaSuccess() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com",
                IsDelete = false
            };
            var contactoEliminado = contacto with { DeleteAt = DateTime.Today, IsDelete = true };

            _repository.Setup(r => r.GetById(contacto.Id)).Returns(Result.Success<Contacto, DomainError>(contacto));
            _repository.Setup(r => r.Delete(contacto.Id)).Returns(Result.Success<Contacto, DomainError>(contactoEliminado));

            //Act
            var res = _service.Delete(contacto.Id);

            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.IsDelete.Should().BeTrue();
            res.Value.DeleteAt.Should().NotBeNull();

            //Verify
            _repository.Verify(r => r.GetById(contacto.Id), Times.Once);
            _repository.Verify(r => r.Delete(contacto.Id), Times.Once);
            _cache.Verify(c => c.Remove(contacto.Id), Times.Once);
        }
    }

    [TestFixture]
    public class CasosInvalidos : ContactoServiceTests {

        private Mock<IContactoRepository> _repository = null!;
        private Mock<ICache<int, Contacto>> _cache = null!;
        private ContactoService _service = null!;

        [SetUp]
        public void SetUp() {
            _repository = new Mock<IContactoRepository>();
            _cache = new Mock<ICache<int, Contacto>>();
            _service = new ContactoService(_repository.Object, _cache.Object);
        }

        [Test]
        public void GetAll_SinContactos_RetornaVacio() {
            //Arrange
            _repository.Setup(r => r.GetAll(1, 5)).Returns(new List<Contacto>());

            //Act
            var res = _service.GetAll();

            //Assert
            res.Should().NotBeNull();
            res.Should().BeEmpty();

            //Verify
            _repository.Verify(r => r.GetAll(1, 5), Times.Once);
        }

        [Test]
        public void GetById_ContactoNoExiste_RetornaFallo() {
            //Arrange
            var error = RepositoryErrors.IdNotFound(66);

            _cache.Setup(c => c.Get(66)).Returns((Contacto?)null);
            _repository.Setup(r => r.GetById(66)).Returns(Result.Failure<Contacto, DomainError>(error));

            //Act
            var res = _service.GetById(66);

            //Assert
            res.IsFailure.Should().BeTrue();

            //Verify
            _cache.Verify(c => c.Get(66), Times.Once);
            _repository.Verify(r => r.GetById(66), Times.Once);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
        }

        [Test]
        public void GetByAlias_ContactoNoExiste_RetornaFallo() {
            //Arrange
            var error = RepositoryErrors.AliasNotFound("Desconocido");

            _repository.Setup(r => r.GetByAlias("Desconocido")).Returns(Result.Failure<Contacto, DomainError>(error));

            //Act
            var res = _service.GetByAlias("Desconocido");

            //Assert
            res.IsFailure.Should().BeTrue();

            //Verify
            _repository.Verify(r => r.GetByAlias("Desconocido"), Times.Once);
        }

        [Test]
        public void Create_RepositorioErroneo_RetornaFallo() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            var error = RepositoryErrors.CreationError();

            _repository.Setup(r => r.Create(contacto)).Returns(Result.Failure<Contacto, DomainError>(error));

            //Act
            var res = _service.Create(contacto);

            //Assert
            res.IsFailure.Should().BeTrue();

            //Verify
            _repository.Verify(r => r.Create(contacto), Times.Once);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
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

            _repository.Setup(r => r.GetById(66)).Returns(Result.Failure<Contacto, DomainError>(RepositoryErrors.IdNotFound(66)));

            //Act
            var res = _service.Update(66, contacto);

            //Assert
            res.IsFailure.Should().BeTrue();

            //Verify
            _repository.Verify(r => r.GetById(66), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
            _cache.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
        }

        [Test]
        public void Update_RepositorioErroneo_RetornaFallo() {
            //Arrange
            var contactoExistente = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            var contactoNuevo = new Contacto {
                Id = 1,
                Alias = "Laura",
                Nombre = "Laura Ruiz Garcia",
                Telefono = "699999999",
                Email = "laura.ruiz@gmail.com"
            };

            _repository.Setup(r => r.GetById(1)).Returns(Result.Success<Contacto, DomainError>(contactoExistente));
            _repository.Setup(r => r.Update(1, contactoNuevo)).Returns(Result.Failure<Contacto, DomainError>(RepositoryErrors.UpdatingError()));

            //Act
            var res = _service.Update(1, contactoNuevo);

            //Assert
            res.IsFailure.Should().BeTrue();

            //Verify
            _repository.Verify(r => r.GetById(1), Times.Once);
            _repository.Verify(r => r.Update(1, contactoNuevo), Times.Once);
            _cache.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
        }

        [Test]
        public void Delete_ContactoInexistente_RetornaFallo() {
            //Arrange
            _repository.Setup(r => r.GetById(66)).Returns(Result.Failure<Contacto, DomainError>(RepositoryErrors.IdNotFound(66)));

            //Act
            var res = _service.Delete(66);

            //Assert
            res.IsFailure.Should().BeTrue();

            //Verify
            _repository.Verify(r => r.GetById(66), Times.Once);
            _repository.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            _cache.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void Delete_RepositorioErroneo_RetornaFallo() {
            //Arrange
            var contacto = new Contacto {
                Id = 1,
                Alias = "Laurita",
                Nombre = "Laura Ruiz",
                Telefono = "666666666",
                Email = "laura@gmail.com"
            };

            _repository.Setup(r => r.GetById(1)).Returns(Result.Success<Contacto, DomainError>(contacto));
            _repository.Setup(r => r.Delete(1)).Returns(Result.Failure<Contacto, DomainError>(RepositoryErrors.DeletingError()));

            //Act
            var res = _service.Delete(1);

            //Assert
            res.IsFailure.Should().BeTrue();

            //Verify
            _repository.Verify(r => r.GetById(1), Times.Once);
            _repository.Verify(r => r.Delete(1), Times.Once);
            _cache.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
        }
    }
}