# 35: Implement hypot

**What to build:** Correctly rounded sqrt(x*x + y*y) under section 5.6, without intermediate posit rounding.

**Blocked by:** 30: Certify sqrt.

**Status:** ready-for-agent

- [ ] Cover E2 8/16/32 and Posit32; preserve sign symmetry, exact zero, and NaR propagation even when the other input is zero.
- [ ] Evaluate exact/certified squares, sum, and root with one final section 4.1 rounding; avoid intermediate overflow/underflow and rounded public sqrt composition.
- [ ] Establish sufficient intermediate storage and finite hardware root/rounding bounds.
- [ ] Independently exhaust posit8 pairs; test wide extreme scale differences, exact Pythagorean cases, saturation, and difficult rounding.
- [ ] Supply synthesized hardware entry points and compare independent raw vectors in simulation/device execution.
- [ ] Update generation, public fusion contract, correctness argument, and coverage matrix.
