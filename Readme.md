# Unum - Proof of concept readme



This project was developed as part of [Hastlayer](https://hastlayer.com/), the .NET HLS tool that converts .NET programs into equivalent logic hardware implementations.

Its goal is to implement a [Unum](http://www.johngustafson.net/unums.html) proof of concept: the number type and an example using it, all transformable with Hastlayer.

The project's source is available in two public source repositories, automatically mirrored in both directions with [Git-hg Mirror](https://githgmirror.com):

- [https://bitbucket.org/Lombiq/arithmetics](https://bitbucket.org/Lombiq/arithmetics) (Mercurial repository)
- [https://github.com/Lombiq/Arithmetics](https://github.com/Lombiq/Arithmetics) (Git repository)

Bug reports, feature requests and comments are warmly welcome, **please do so via GitHub**. Feel free to send pull requests too, no matter which source repository you choose for this purpose.

This project is developed by [Lombiq Technologies Ltd](https://lombiq.com/). Commercial-grade support is available through Lombiq.


## About Unum

Unum is a new number format invented by Dr. John L. Gustafson that can be used to store any number with exact precision (or known error). It can be used to achieve better range and accuracy than IEEE floating point formats while eliminating the algebraic errors that the IEEE floats are prone to.

For more about its advantages see: [http://ubiquity.acm.org/article.cfm?id=2913029](http://ubiquity.acm.org/article.cfm?id=2913029).