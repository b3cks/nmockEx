Caveats:

- No documentation yet.  Read the acceptance tests to see how to use the library.
- Cannot mock classes yet.

Changes:
- Added Fire Syntax for Event handling
- Fixed ordered expectations
- Added Has.Property(string PropertyName, Matcher propertyMatcher) matcher
- Introduced ExpectationException and replaced all uses of AssertionException
- Fixed ordered calls when using "at least..." and "between..." expectations
- Renamed namespaces and build paths to contain NMock2, not SharpMock
- Removed OO-Matron stuff from SharpMock.Monitoring namespace
- Removed IExpectationCollection interface
- Renamed AssertAllExpectationsHaveBeenMet to VerifyAllExpectationsHaveBeenMet
- Added Verify.That assertion function that uses Matchers
- Moved internal interfaces and classes into SharpMock.Internal namespace

Todo:

- More Documentation
- A Mockery that can mock classes (use NMock 1 bytecode generator?)
- More matchers: Has.Property(string PropertyName, Matcher propertyMatcher), etc.
- Default result stub to synthesise results based on return type [in progress]
- Distinguish pure stubs from mocks: have a Mockery::NewStub method that
  returns a stub object that cannot be only used to set up stubbed methods, 
  not expectations, and returns default values for methods that do not have
  explicit stubs.

Questions:
- Should I get rid of the ISelfDescribing interface and just put a DescribeTo 
  method in each interface that currently extends it?  Pro: ISelfDescribing is
  not used directly.  Con: duplicated declarations of DescribeTo.
