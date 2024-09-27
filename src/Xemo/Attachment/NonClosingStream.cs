/// <summary>
/// Stream which ignores any attempts to close it.
/// </summary>
public class NonClosingStream(Stream origin) : Stream
{
    public override bool CanRead => origin.CanRead;
    public override bool CanSeek => origin.CanSeek;
    public override bool CanWrite => origin.CanWrite;
    public override long Length => origin.Length;

    public override long Position
    {
        get => origin.Position;
        set => origin.Position = value;
    }

    public override void Flush() => origin.Flush();
    public override int Read(byte[] buffer, int offset, int count) => origin.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin seekOrigin) => origin.Seek(offset, seekOrigin);
    public override void SetLength(long value) => origin.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => origin.Write(buffer, offset, count);
    protected override void Dispose(bool disposing)
    {
    }
}