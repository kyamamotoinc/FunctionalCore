using FunctionalCore.Extensions;
using NUnit.Framework.Legacy;

namespace FunctionalCore.Tests.OptionTests.Extensions
{
    public class OptionTraverseTests
    {
        private IReadOnlyList<int> _items;

        [SetUp]
        public void Setup()
        {
            _items = new[] { 1, 2, 3 };
        }

        /// <summary>
        /// 1. すべての要素が Some に変換される場合、値をまとめた Some(List) を返す
        /// </summary>
        [Test]
        public void Traverse_all_items_return_Some_should_return_Some_list()
        {
            var result = _items.Traverse(x => Option<int>.Some(x * 2));

            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 2, 4, 6 }));
        }

        /// <summary>
        /// 2. 途中で None が返された場合、None を返す
        /// </summary>
        [Test]
        public void Traverse_any_item_returns_None_should_return_None()
        {
            var result = _items.Traverse(x =>
                x == 2
                    ? Option<int>.None
                    : Option<int>.Some(x * 2));

            Assert.That(result.HasValue, Is.False);
        }

        /// <summary>
        /// 3. 空コレクションの場合、空リストを持つ Some を返す
        /// </summary>
        [Test]
        public void Traverse_empty_collection_should_return_Some_empty_list()
        {
            var items = Array.Empty<int>();

            var result = items.Traverse(x => Option<int>.Some(x * 2));

            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.Empty);
        }

        /// <summary>
        /// 4. f が null の場合、ArgumentNullException を投げる
        /// </summary>
        [Test]
        public void Traverse_null_function_should_throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = _items.Traverse<int, int>(null!));
        }

        /// <summary>
        /// 5. 値の順序は保持される
        /// </summary>
        [Test]
        public void Traverse_should_preserve_order()
        {
            var items = new[] { 3, 1, 2 };

            var result = items.Traverse(x => Option<string>.Some(x.ToString()));

            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { "3", "1", "2" }));
        }

        /// <summary>
        /// 6. None が返された時点で以降の要素は評価されない
        /// </summary>
        [Test]
        public void Traverse_should_stop_after_None()
        {
            var count = 0;

            var result = _items.Traverse(x =>
            {
                count++;

                return x == 2
                    ? Option<int>.None
                    : Option<int>.Some(x);
            });

            Assert.That(result.HasValue, Is.False);
            Assert.That(count, Is.EqualTo(2));
        }
    }
}
