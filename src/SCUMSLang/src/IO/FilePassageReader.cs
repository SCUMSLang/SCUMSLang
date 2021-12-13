using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SCUMSLang.Text;
using Teronis.IO.FileLocking;

namespace SCUMSLang.IO
{
    public class FilePassageReader
    {
        public static FilePassageReader Default = new FilePassageReader();

        public async Task<List<string>?> ReadPassageAsync(string filePath, int position, int length)
        {
            if (FileStreamLocker.Default.TryAcquire(filePath, out var fileStream, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                try {
                    var fileStreamLength = fileStream.Length;
                    var positionLength = position + length;
                    var beginPosition = position;

                    if (beginPosition > 0) {
                        do {
                            fileStream.Seek(beginPosition - 1, SeekOrigin.Begin);
                            var previousByte = (byte)fileStream.ReadByte();

                            if (previousByte == '\r' || previousByte == '\n') {
                                break;
                            }

                            beginPosition--;
                        } while (beginPosition > 0);
                    }

                    var relativePosition = position - beginPosition;
                    var relativePositionLength = relativePosition + length;

                    var endPosition = position + length;

                    do {
                        fileStream.Seek(endPosition, SeekOrigin.Begin);
                        var nextByte = (byte)fileStream.ReadByte();

                        if (nextByte == '\r' || nextByte == '\n') {
                            break;
                        }

                        endPosition++;
                    } while (endPosition < fileStreamLength);

                    fileStream.Seek(beginPosition, SeekOrigin.Begin);

                    var fileBytes = new byte[endPosition - beginPosition];
                    await fileStream.ReadAsync(fileBytes).ConfigureAwait(false);
                    var fileBytesMemory = new ReadOnlyMemory<byte>(fileBytes);

                    if (beginPosition < EncodingTools.UTF8Preamble.Length) {
                        fileBytesMemory = EncodingTools.CutOffUTF8Preamble(fileBytesMemory);
                    }

                    var utf8Content = Encoding.UTF8.GetString(fileBytesMemory.Span);
                    var utf8ContentLength = utf8Content.Length;
                    var lineList = new List<string>();
                    var utf8ContentIndexOfNextLine = 0;
                    var currentIndex = 0;
                    var detectedEndOfLine = '\0';
                    var skippableEndOfLine = '\0';

                    var lineListIndexOfPosition = -1;
                    int linePositionOfPosition = -1;
                    var lineListIndexOfPositionLength = -1;
                    int linePositionOfPositionLength = -1;

                    void sliceNextLine()
                    {
                        if (lineListIndexOfPosition == -1) {
                            if (currentIndex >= relativePosition) {
                                lineListIndexOfPosition = lineList.Count;
                                linePositionOfPosition = relativePosition - utf8ContentIndexOfNextLine;
                            }
                        } else if (lineListIndexOfPositionLength == -1) {
                            if (currentIndex >= relativePositionLength) {
                                linePositionOfPositionLength = relativePositionLength - utf8ContentIndexOfNextLine;

                                // If the position at position+length is \r\n, \n, \r ..
                                if (linePositionOfPositionLength <= 0) {
                                    // .. find previous line with at least one character.
                                    lineListIndexOfPositionLength = lineList.FindLastIndex(x => x.Length != 0);
                                    linePositionOfPositionLength = lineList[lineListIndexOfPositionLength].Length - 1;
                                } else {
                                    lineListIndexOfPositionLength = lineList.Count;
                                }
                            }
                        }

                        lineList.Add(utf8Content.Substring(utf8ContentIndexOfNextLine, currentIndex - utf8ContentIndexOfNextLine));
                        utf8ContentIndexOfNextLine = currentIndex;
                    }

                    while (currentIndex < utf8ContentLength) {


                        var character = fileBytesMemory.Span[currentIndex];

                        if (detectedEndOfLine != '\0') {
                            if (character == skippableEndOfLine) {
                                // We do not want \n of \r\n in next line.
                                // But: Mixed line endings will screw up.
                                utf8ContentIndexOfNextLine = currentIndex + 1;
                            } else if (character == detectedEndOfLine) {
                                sliceNextLine();
                            }
                        } else {
                            if (character == '\r') {
                                detectedEndOfLine = '\r';
                                skippableEndOfLine = '\n';
                                sliceNextLine();
                            } else if (character == '\n') {
                                detectedEndOfLine = '\n';
                                skippableEndOfLine = '\r';
                                sliceNextLine();
                            }
                        }

                        currentIndex++;
                    }

                    sliceNextLine();

                    if (lineList.Count == 0) {
                        lineList.Add(utf8Content);
                    }

                    if (lineListIndexOfPosition == -1) {
                        lineListIndexOfPosition = 0;
                        linePositionOfPosition = relativePosition;
                    }

                    if (lineListIndexOfPositionLength == -1) {
                        lineListIndexOfPositionLength = lineList.FindLastIndex(x => x.Length != 0);
                        linePositionOfPositionLength = lineList[lineListIndexOfPositionLength].Length - 1;
                    }

                    var stringBuilder = new StringBuilder();

                    for (var currentLineIndex = lineListIndexOfPositionLength; currentLineIndex >= lineListIndexOfPosition; currentLineIndex--) {
                        stringBuilder.Clear();

                        if (currentLineIndex == lineListIndexOfPosition) {
                            stringBuilder.Append(new string(' ', linePositionOfPosition));

                            var underlinedLength = length;
                            var maxUnderlinableLength = lineList[currentLineIndex].Length - linePositionOfPosition;

                            if (underlinedLength > maxUnderlinableLength) {
                                underlinedLength = maxUnderlinableLength;
                            }

                            stringBuilder.Append(new string('^', underlinedLength));
                        } else if (currentLineIndex == lineListIndexOfPositionLength) {
                            stringBuilder.Append(new string('^', linePositionOfPositionLength));
                            stringBuilder.Append(new string(' ', lineList[currentLineIndex].Length - linePositionOfPositionLength));
                        } else {
                            stringBuilder.Append(new string('^', lineList[currentLineIndex].Length));
                        }

                        lineList.Insert(currentLineIndex + 1, stringBuilder.ToString());
                    }

                    return lineList;
                } catch {
                    goto exit;
                } finally {
                    fileStream.Dispose();
                }
            }

            exit:
            return null;
        }

        public Task<List<string>?> ReadPassage(string filePath, int position) =>
            ReadPassageAsync(filePath, position, length: 1);
    }
}
