using FunctionalCore.Extensions;
using NUnit.Framework.Legacy;

namespace FunctionalCore.Tests.ResultTests.Extensions
{
    public class ResultSequenceTests
    {
        private Result<int, int> _some1;
        private Result<int, int> _some2;
        private Result<int, int> _some3;
        private Result<int, int> _none;

        [SetUp]
        public void Setup()
        {
            _some1 = Result<int, int>.Ok(1);
            _some2 = Result<int, int>.Ok(2);
            _some3 = Result<int, int>.Ok(3);
            _none = Result<int, int>.Fail(0);
        }

        /// <summary>
        /// 1. すべて Ok の場合、値をまとめた Ok(Collection) を返す
        /// </summary>
        [Test]
        public void Sequence_all_Ok_should_return_Ok_collection()
        {
            var results = new[]
            {
                _some1,
                _some2,
                _some3
             };

            var result = results.Sequence();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        /// <summary>
        /// 2. None を含む場合、None を返す
        /// </summary>
        [Test]
        public void Sequence_contains_None_should_return_None()
        {
            var results = new[]
            {
                _some1,
                _none,
                _some3
             };

            var result = results.Sequence();

            Assert.That(result.IsSuccess, Is.False);
        }

        /// <summary>
        /// 3. 空コレクションの場合、空コレクションを持つ Some を返す
        /// </summary>
        [Test]
        public void Sequence_empty_collection_should_return_Some_empty_collection()
        {
            var options = Array.Empty<Option<int>>();

            var result = options.Sequence();

            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        /// <summary>
        /// 4. null のコレクションを渡した場合、ArgumentNullException を投げる
        /// </summary>
        [Test]
        public void Sequence_null_options_should_throw_ArgumentNullException()
        {
            IEnumerable<Option<int>> options = null!;

            Assert.Throws<ArgumentNullException>(() => options.Sequence());
        }

        /// <summary>
        /// 5. 値の順序は保持される
        /// </summary>
        [Test]
        public void Sequence_should_preserve_order()
        {
            var results = new[]
            {
                _some3,
                _some1,
                _some2
             };

            var result = results.Sequence();

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 3, 1, 2 }));
        }
    }
}
