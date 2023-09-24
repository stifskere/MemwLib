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

// Method type supports flags and path supports Regex c# api.
server.AddEndpoint(RequestMethodType.Get | RequestMethodType.Put, "/test", request => {
    return new ResponseEntity(200, "Hello World!"); 
    // (int code, string httpVersion, string body) 
    // (int code, string body)
});

// thread blocking due to implementation.
```

### Colors

Rgb color implementation in 2 classes `<Rgb24>` and `<Rgb32>`, for future usage in other library parts.

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

Both classes can be casted into each-other, and both classes can be casted into a `<uint>`.

```csharp
uint red = (uint)new Rgb24(255, 0, 0);
uint green = (uint)new Rgb32(0, 255, 0, 255);
```