using NUnit.Framework;
using System;

namespace FunctionalCore.Tests.OptionTests
{
    public class DefaultOptionTests
    {
        private Option<string> _default;

        [SetUp]
        public void Setup()
        {
            _default = default(Option<string>);
        }

        /// <summary>
        /// 1. Default Option は None と等しい
        /// </summary>
        [Test]
        public void Default_Option_should_equal_None()
        {
            Assert.That(_default, Is.EqualTo(Option<string>.None));
        }

        /// <summary>
        /// 2. Default Option は値を持たない
        /// </summary>
        [Test]
        public void Default_Option_should_not_have_value()
        {
            Assert.IsFalse(_default.HasValue);
        }

        /// <summary>
        /// 3. Default Option では Value にアクセスできない
        /// </summary>
        [Test]
        public void Default_Option_accessing_Value_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() => _ = _default.Value);
        }

        /// <summary>
        /// 4. Default Option の Map は None を返す
        /// </summary>
        [Test]
        public void Default_Option_Map_should_return_None()
        {
            var result = _default.Map(x => x.Length);
            Assert.That(result, Is.EqualTo(Option<int>.None));
        }

        /// <summary>
        /// 5. Default Option の Bind は None を返す
        /// </summary>
        [Test]
        public void Default_Option_Bind_should_return_None()
        {
            var result = _default.Bind(x => Option<int>.Some(x.Length));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        }

        /// <summary>
        /// 6. Default Option の Match は onNone を実行する
        /// </summary>
        [Test]
        public void Default_Option_Match_should_invoke_onNone()
        {
            var result = _default.Match(
                onSome: x => "some",
                onNone: () => "none"
            );
            Assert.That(result, Is.EqualTo("none"));
        }

        /// <summary>
        /// 7. Default Option の void Match は onNone を実行する
        /// </summary>
        [Test]
        public void Default_Option_void_Match_should_invoke_onNone()
        {
            var invoked = false;
            _default.Match(
                onSome: x => { },
                onNone: () => { invoked = true; }
            );
            Assert.That(invoked);
        }

        /// <summary>
        /// 8. Default Option の Ensure は None を返す
        /// </summary>
        [Test]
        public void Default_Option_Ensure_should_return_None()
        {
            var result = _default.Ensure(x => true);
            Assert.That(result, Is.EqualTo(Option<string>.None));
        }

        /// <summary>
        /// 9. Default Option の Tap は実行されない
        /// </summary>
        [Test]
        public void Default_Option_Tap_should_not_invoke_action()
        {
            var invoked = false;
            _default.Tap(x => { invoked = true; });
            Assert.IsFalse(invoked);
        }

        /// <summary>
        /// 10. Default Option の TapNone は実行される
        /// </summary>
        [Test]
        public void Default_Option_TapNone_should_invoke_action()
        {
            var invoked = false;
            _default.TapNone(() => { invoked = true; });
            Assert.That(invoked);
        }

        /// <summary>
        /// 11. Default Option の ToString は "None" を返す
        /// </summary>
        [Test]
        public void Default_Option_ToString_should_return_None()
        {
            Assert.That(_default.ToString(), Is.EqualTo("None"));
        }

        /// <summary>
        /// 12. 配列初期化も None として動作する
        /// </summary>
        [Test]
        public void Array_initialized_Option_should_equal_None()
        {
            var options = new Option<string>[1];
            Assert.That(options[0], Is.EqualTo(Option<string>.None));
        }
    }
}
