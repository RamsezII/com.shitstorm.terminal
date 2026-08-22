using NUnit.Framework;
using System.IO;

namespace _TERMINAL_.Tests
{
    public sealed class LineParserTests
    {
        [SetUp]
        public void SetUp() => LineParser.ResetCompletion();

        [Test]
        public void ManualAndAltUseDifferentBits()
        {
            Assert.That((CmdM.Man & CmdM.Alt), Is.EqualTo((CmdM)0));
        }

        [Test]
        public void FirstTabUsesFirstCompletion()
        {
            LineParser line = new("t", Directory.GetCurrentDirectory(), CmdM.Tab, 1);
            string prefix = line.Read();

            Assert.That(line.IsCplThis, Is.True);
            Assert.That(line.OnCpls(prefix, "two", "three"), Is.True);
            Assert.That(line.rawtext, Is.EqualTo("two"));
        }

        [Test]
        public void EmptyQuotedPathCompletionDoesNotIndexBeforeInput()
        {
            LineParser line = new("\"\"", Directory.GetCurrentDirectory(), CmdM.Tab, 1);

            Assert.DoesNotThrow(() => line.TryReadAsPath(out _));
        }

        [Test]
        public void BinaryRoundTripPreservesHighCommandFlags()
        {
            LineParser source = new(
                "test",
                Directory.GetCurrentDirectory(),
                CmdM.Exec | CmdM._telepathy,
                4);

            using MemoryStream stream = new();
            using (BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, leaveOpen: true))
                source.WriteBytes(writer);

            stream.Position = 0;
            using BinaryReader reader = new(stream);
            LineParser restored = LineParser.ReadBytes(reader);

            Assert.That(restored.cmdM, Is.EqualTo(source.cmdM));
        }

        [Test]
        public void ForcedEmptyArgumentIsQuoted()
        {
            Assert.That(string.Empty.Quotes(force: true), Is.EqualTo("\"\""));
        }
    }
}
