using FunctionalCore.Extensions;
using NUnit.Framework.Legacy;

namespace FunctionalCore.Tests.OptionTests.Extensions
{
    public class OptionSequenceTests
    {
        private Option<int> _some1;
        private Option<int> _some2;
        private Option<int> _some3;
        private Option<int> _none;

        [SetUp]
        public void Setup()
        {
            _some1 = Option<int>.Some(1);
            _some2 = Option<int>.Some(2);
            _some3 = Option<int>.Some(3);
            _none = Option<int>.None;
        }

        /// <summary>
        /// 1. すべて Some の場合、値をまとめた Some(Collection) を返す
        /// </summary>
        [Test]
        public void Sequence_all_Some_should_return_Some_collection()
        {
            var options = new[]
            {
            _some1,
            _some2,
            _some3
        };

            var result = options.Sequence();

            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        /// <summary>
        /// 2. None を含む場合、None を返す
        /// </summary>
        [Test]
        public void Sequence_contains_None_should_return_None()
        {
            var options = new[]
            {
            _some1,
            _none,
            _some3
        };

            var result = options.Sequence();

            Assert.That(result.HasValue, Is.False);
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
            var options = new[]
            {
            _some3,
            _some1,
            _some2
        };

            var result = options.Sequence();

            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 3, 1, 2 }));
        }
    }
}
