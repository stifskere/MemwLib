# MemwLib

Utils library to mostly cover needs or *better implementations* of needs.

> This library is meant to be expanded with ideas over time.


## Features

- Http server
- Colors

### Http server

It's an implementation for the overly complicated c# webserver, it's made to resemble the npm package [express.js](https://expressjs.com/)

Basic usage demonstration:
```csharp
HttpServer server = new(IpAdress.Any, 3000);
// (IpAdress ip, ushort port)
// (IpAdress ip, ushort port, CancellationToken cancellationToken)

server.AddEndpoint(RequestMethodType.Get | RequestMethodType.Put, "/test", request => {
    return new ResponseEntity(200, "Hello World!"); 
    // (int code, string httpVersion, string body) 
    // (int code, string body)
});
// (RequestMethodType method, string path, EndpointHandlerDelegate handler)
// (RequestMethodType method, Regex path, EndpointHandlerDelegate handler)
```

Notice how using an HttpServer instance will block the running thread, it is advised to use a CancellationToken to unblock the thread and stop the server.

### Colors

Rgb color implementation in 4 classes `<Rgb24>`, `<Rgb32>`, `<Rgb48>` and `<Rgb64>`, for future usage in other library parts.

Basic usage demonstration:
```csharp
// 3 bytes for red, green, blue
Rgb24 red = (Rgb24)0xFF0000;
Rgb24 green = new Rgb24(0x00FF00);
Rgb24 blue = new Rgb24(0, 0, 255);
```

```csharp
// last byte represents opacity.
Rgb32 red = (Rgb32)0xFF0000FF;
Rgb32 green = new Rgb32(0x00FF00FF);
Rgb32 blue = new Rgb32(0, 0, 255, 255);
```
24 and 32 byte colors have no correlation with 48 and 64 bytes and cannot be casted with eachother.
```csharp
// 6 bytes for red, green and blue
Rgb48 Red = (Rgb48)0xffff0000;
Rgb48 Green = new Rgb48(0xff008000);
Rgb48 blue = new Rgb48(0, 0, 65535);
```
```csharp
// last 2 bytes represent opacity.
Rgb64 Red = (Rgb64)0xffff00000000;
Rgb64 Green = new Rgb64(0xff0080000000);
Rgb64 Blue = new Rgb64(0x00000000ffff);
```
For 24 and 32 bit types:
- `<Rgb32>` can be explicitly cast to `<Rgb24>`, but not vice versa.
- Both `<Rgb32>` and `<Rgb24>` types can be explicitly cast to `<uint>`.
- `<uint>` can be explicitly cast to both `<Rgb32>` and `<Rgb24>`.

For 48 and 64 bit types:

- `<Rgb64>` can be explicitly cast to `<Rgb48>`, but not vice versa.
- Both `<Rgb64>` and `<Rgb48>` types can be explicitly cast to `<ulong>`.
- `<ulong>` can be explicitly cast to both `<Rgb64>` and `<Rgb48>`.

Note that casting from higher bit types to lower bit types result in a data loss.

```csharp
uint red = (uint)new Rgb24(255, 0, 0);
uint green = (uint)new Rgb32(0, 255, 0, 255);

Rgb24 blue = (Rgb24)new Rgb32(0, 0, 255, 255); // opacity will be lost.
```
```csharp
ulong red = (ulong)new Rgb48(65535, 0, 0);
ulong green = (ulong)new Rgb64(0, 65535, 0, 65535);

Rgb48 blue = (Rgb48)new Rgb64(0, 0, 65535, 65535); // opacity will be lost.
```
