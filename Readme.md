# Lombiq Arithmetics readme



[Next-generation arithmetic](https://posithub.org/) implementations, improved floating point number types for .NET, written in C#. Includes the following number types:

- Posit: a drop-in replacement for standard `float`s and `double`s that provides more accurate results with fewer bits. Can be used in any .NET software that needs better accuracy than `double`s. For more info see the "[Beating Floating Point at its Own Game: Posit Arithmetic](http://www.johngustafson.net/pdfs/BeatingFloatingPoint.pdf)" white paper.
- Unum: similar to posit but with a more complex implementation, a prior idea; just a basic proof of concept. For more info see the [compilation of unum resources](](http://www.johngustafson.net/unums.html) on Dr. John Gustafson's website.

This project was developed as part of [Hastlayer](https://hastlayer.com/), the .NET HLS tool that converts .NET programs into equivalent logic hardware implementations. Both the posit and unum implementation can be automatically converted into FPGA-implemented logic hardware.

We've written a detailed whitepaper about our posit implementation and its results. You can download the whitepaper for free by visiting the link found on our [Next Generation Arithmetic with Hastlayer page](https://hastlayer.com/arithmetics).

This project is developed by [Lombiq Technologies Ltd](https://lombiq.com/). Commercial-grade support is available through Lombiq.


## About unum

Unum is a new number format invented by Dr. John L. Gustafson that can be used to store any number with exact precision (or known error). It can be used to achieve better range and accuracy than IEEE floating point formats while eliminating the algebraic errors that the IEEE floats are prone to.

For more about its advantages see: [http://ubiquity.acm.org/article.cfm?id=2913029](http://ubiquity.acm.org/article.cfm?id=2913029).


## Contributing

The project's source is available in two public source repositories, automatically mirrored in both directions with [Git-hg Mirror](https://githgmirror.com):

- [https://bitbucket.org/Lombiq/arithmetics](https://bitbucket.org/Lombiq/arithmetics) (Mercurial repository)
- [https://github.com/Lombiq/Arithmetics](https://github.com/Lombiq/Arithmetics) (Git repository)

Bug reports, feature requests and comments are warmly welcome, **please do so via GitHub**. Feel free to send pull requests too, no matter which source repository you choose for this purpose.