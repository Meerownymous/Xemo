using System;
using System.IO;
using System.Threading.Tasks;

public class BufferingStream : Stream
{
    private readonly Stream memory;
    private readonly MemoryStream origin;
    private long _bufferedUntil = 0; // Tracks how much has been buffered

    public BufferingStream(Stream innerStream, MemoryStream memory)
    {
        this.memory = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
        origin = memory;
    }

    public override bool CanRead => memory.CanRead;
    public override bool CanSeek => memory.CanSeek;
    public override bool CanWrite => memory.CanWrite;
    public override long Length => memory.Length;

    public override long Position
    {
        get => memory.Position;
        set
        {
            memory.Position = value;
            origin.Position = Math.Min(value, _bufferedUntil); // Sync buffer position but don't exceed buffered data
        }
    }

    public override void Flush()
    {
        memory.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        // Step 1: Read from buffer if the position is within the buffered range
        if (Position < _bufferedUntil)
        {
            // Set the buffer stream to the current position
            origin.Position = Position;

            // Determine how many bytes we can read from the buffer
            int bytesAvailableInBuffer = (int)(_bufferedUntil - Position);
            int bytesToReadFromBuffer = Math.Min(bytesAvailableInBuffer, count);

            // Read from the buffer
            int bytesReadFromBuffer = origin.Read(buffer, offset, bytesToReadFromBuffer);
            totalBytesRead += bytesReadFromBuffer;
            Position += bytesReadFromBuffer;

            // If we read all requested bytes from the buffer, return
            if (totalBytesRead == count)
            {
                return totalBytesRead;
            }

            // Adjust the offset and count to reflect the remaining bytes we still need to read
            offset += bytesReadFromBuffer;
            count -= bytesReadFromBuffer;
        }

        // Step 2: Read any remaining bytes from the inner stream
        if (count > 0)
        {
            int bytesReadFromStream = memory.Read(buffer, offset, count);
            if (bytesReadFromStream > 0)
            {
                // Write to the buffer what was just read from the inner stream
                origin.Position = _bufferedUntil; // Write at the end of the buffered data
                origin.Write(buffer, offset, bytesReadFromStream);

                // Update the amount of data we've buffered
                _bufferedUntil += bytesReadFromStream;
                totalBytesRead += bytesReadFromStream;
            }
        }

        return totalBytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        origin.Position = memory.Position;
        memory.Write(buffer, offset, count);
        origin.Write(buffer, offset, count);
        _bufferedUntil = Math.Max(_bufferedUntil, Position);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPosition = memory.Seek(offset, origin);
        this.origin.Position = Math.Min(newPosition, _bufferedUntil);
        return newPosition;
    }

    public override void SetLength(long value)
    {
        memory.SetLength(value);
        origin.SetLength(value); // Sync buffer length
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            memory?.Dispose();
            origin?.Dispose();
        }

        base.Dispose(disposing);
    }
}
