using Quixotic.Common.Source;

namespace Quixotic.CommonTests.Source
{
    [TestClass]
    public class SourceDocumentTests
    {
        [TestMethod]
        public void Lines()
        {
            // Setup
            var source =
                "####\n" +
                "\n" +
                "####\n" +
                "####";

            // Execute
            var sourceDocument = new SourceDocument(source);

            // Assert
            Assert.HasCount(4, sourceDocument.Lines);

            var line = sourceDocument.Lines[0];
            Assert.AreEqual(0, line.Start);
            Assert.AreEqual(5, line.Length);
            Assert.AreEqual(4, line.End);
            Assert.AreEqual(1, line.LineNumber);
            Assert.AreEqual(1, line.StartPosition.Line);
            Assert.AreEqual(1, line.StartPosition.Column);
            Assert.AreEqual(line.Start, line.StartPosition.Index);
            Assert.AreEqual(1, line.EndPosition.Line);
            Assert.AreEqual(5, line.EndPosition.Column);
            Assert.AreEqual(line.End, line.EndPosition.Index);
            Assert.AreEqual("####\n", line.Text);

            line = sourceDocument.Lines[1];
            Assert.AreEqual(5, line.Start);
            Assert.AreEqual(1, line.Length); // Just the new line character
            Assert.AreEqual(5, line.End);
            Assert.AreEqual(2, line.LineNumber);
            Assert.AreEqual(2, line.StartPosition.Line);
            Assert.AreEqual(1, line.StartPosition.Column);
            Assert.AreEqual(line.Start, line.StartPosition.Index);
            Assert.AreEqual(2, line.EndPosition.Line);
            Assert.AreEqual(1, line.EndPosition.Column);
            Assert.AreEqual(line.End, line.EndPosition.Index);
            Assert.AreEqual("\n", line.Text);

            line = sourceDocument.Lines[2];
            Assert.AreEqual(6, line.Start);
            Assert.AreEqual(5, line.Length);
            Assert.AreEqual(10, line.End);
            Assert.AreEqual(3, line.LineNumber);
            Assert.AreEqual(3, line.StartPosition.Line);
            Assert.AreEqual(1, line.StartPosition.Column);
            Assert.AreEqual(line.Start, line.StartPosition.Index);
            Assert.AreEqual(3, line.EndPosition.Line);
            Assert.AreEqual(5, line.EndPosition.Column);
            Assert.AreEqual(line.End, line.EndPosition.Index);
            Assert.AreEqual("####\n", line.Text);

            line = sourceDocument.Lines[3];
            Assert.AreEqual(11, line.Start);
            Assert.AreEqual(4, line.Length);
            Assert.AreEqual(14, line.End);
            Assert.AreEqual(4, line.LineNumber);
            Assert.AreEqual(4, line.StartPosition.Line);
            Assert.AreEqual(1, line.StartPosition.Column);
            Assert.AreEqual(line.Start, line.StartPosition.Index);
            Assert.AreEqual(4, line.EndPosition.Line);
            Assert.AreEqual(4, line.EndPosition.Column);
            Assert.AreEqual(line.End, line.EndPosition.Index);
            Assert.AreEqual("####", line.Text);
        }

        [TestMethod]
        public void GetLineNumber()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetLineNumber(7);

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetColumnNumber()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetColumnNumber(7);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GetIndex()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetIndex(2, 3);

            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void GetPosition()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetPosition(7);

            Assert.IsFalse(result.IsEmpty);
            Assert.AreEqual(2, result.Line);
            Assert.AreEqual(3, result.Column);
            Assert.AreEqual(7, result.Index);
        }

        [TestMethod]
        public void GetLineNumber_with_out_of_range_index()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetLineNumber(25);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void GetColumnNumber_with_out_of_range_index()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetColumnNumber(25);

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void GetIndex_with_line_and_column_of_range_index()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetIndex(6, 6);

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void GetIndex_with_column_of_range_index()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetIndex(2, 6);

            Assert.AreEqual(-1, result);
        }



        [TestMethod]
        public void GetPosition_with_out_of_range_index()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n" +
                "####\n";
            var sourceDocument = new SourceDocument(source);


            // Execute
            var result = sourceDocument.GetPosition(25);

            Assert.IsTrue(result.IsEmpty);
        }

        [TestMethod]
        public void EmptySource()
        {
            // Setup
            var document = new SourceDocument("");

            // Assert
            Assert.HasCount(1, document.Lines);

            var line = document.Lines[0];

            Assert.AreEqual(1, line.LineNumber);
            Assert.AreEqual(0, line.Start);
            Assert.AreEqual(0, line.Length);
            Assert.AreEqual(-1, line.End);
            Assert.AreEqual("", line.Text);

            Assert.IsTrue(document.GetPosition(0).IsEmpty);
        }

        [TestMethod]
        public void TrailingNewLine()
        {
            // Setup
            var source =
                "####\n" +
                "####\n" +
                "####\n";

            var document = new SourceDocument(source);

            // Assert
            Assert.HasCount(3, document.Lines);

            var line = document.Lines[2];

            Assert.AreEqual(10, line.Start);
            Assert.AreEqual(5, line.Length);
            Assert.AreEqual("####\n", line.Text);
        }

        [TestMethod]
        public void ConsecutiveEmptyLines()
        {
            // Setup
            var source =
                "\n" +
                "\n" +
                "\n";

            var document = new SourceDocument(source);

            // Assert
            Assert.HasCount(3, document.Lines);

            foreach (var line in document.Lines)
            {
                Assert.AreEqual("\n", line.Text);
                Assert.AreEqual(1, line.Length);
            }
        }

        [TestMethod]
        public void RoundTrip_AllIndices()
        {
            // Setup
            var source =
                "####\n" +
                "\n" +
                "####\n" +
                "####";

            var document = new SourceDocument(source);

            // Execute / Assert
            for (int i = 0; i < document.Length; i++)
            {
                var position = document.GetPosition(i);

                Assert.IsFalse(position.IsEmpty);

                var reconstructed =
                    document.GetIndex(position.Line, position.Column);

                Assert.AreEqual(i, reconstructed);
            }
        }

        [TestMethod]
        public void BoundaryIndices()
        {
            // Setup
            var source =
                "####\n" +
                "####";

            var document = new SourceDocument(source);

            Assert.AreEqual(1, document.GetLineNumber(0));
            Assert.AreEqual(1, document.GetColumnNumber(0));

            Assert.AreEqual(1, document.GetLineNumber(3));
            Assert.AreEqual(4, document.GetColumnNumber(3));

            Assert.AreEqual(1, document.GetLineNumber(4));
            Assert.AreEqual(5, document.GetColumnNumber(4));

            Assert.AreEqual(2, document.GetLineNumber(5));
            Assert.AreEqual(1, document.GetColumnNumber(5));

            Assert.AreEqual(2, document.GetLineNumber(8));
            Assert.AreEqual(4, document.GetColumnNumber(8));
        }

        [TestMethod]
        public void WindowsNewLines()
        {
            // Setup
            var source =
                "####\r\n" +
                "####\r\n";

            var document = new SourceDocument(source);

            // Assert
            Assert.HasCount(2, document.Lines);

            Assert.AreEqual("####\r\n", document.Lines[0].Text);
            Assert.AreEqual("####\r\n", document.Lines[1].Text);
        }
    }
}
