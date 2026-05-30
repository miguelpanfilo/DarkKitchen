using BusinessLogics.Shared;

  namespace Tests.Users;

  [TestClass]
  public class ValidatorPhoneUruguayTests
  {
      [TestMethod]
      public void IsValid_NullPhone_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          string? phone = null;
          var result = validator.ItsValid(phone!);

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_EmptyPhone_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid(string.Empty);

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_PhoneWithOnlySpaces_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("   ");

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_PhoneShorterThan9_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("09912345");

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_PhoneLongerThan9_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("0991234567");

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_PhoneWithLetters_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("09912A456");

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_PhoneWithSpecialCharacter_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("09912-456");

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_PhoneWithInvalidPrefix_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("090123456");

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_LandlinePhone_ReturnsFalse()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("29012345");

          Assert.IsFalse(result);
      }

      [TestMethod]
      public void IsValid_PhoneWithPrefix091_ReturnsTrue()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("091123456");

          Assert.IsTrue(result);
      }

      [TestMethod]
      public void IsValid_PhoneWithPrefix099_ReturnsTrue()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("099123456");

          Assert.IsTrue(result);
      }

      [TestMethod]
      public void IsValid_PhoneWithPrefix098_ReturnsTrue()
      {
          var validator = new PhoneValidatorUruguay();

          var result = validator.ItsValid("098123456");

          Assert.IsTrue(result);
      }
  }
