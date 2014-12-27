TARGET?=Debug
BUILDDIR=./build/$(TARGET)

LIBDIR=lib
LIBS=System.dll,nunit.framework.dll

MCS=mcs
MCSFLAGS_Debug=-debug --stacktrace
MCSFLAGS_Release=-optimize
MCSFLAGS=-lib:$(LIBDIR) -r:$(LIBS) $(MCSFLAGS_$(TARGET))

NUNIT=mono tools/mono/nunit-console.exe /nologo

NMOCK2_DLL=$(BUILDDIR)/NMock2.dll
NMOCK2_PDB=$(BUILDDIR)/NMock2.pdb
NMOCK2_SRC=$(shell find src/NMock2 -name '*.cs')
NMOCK2_RES=

NMOCK2_TEST_DLL=$(BUILDDIR)/NMock2.Test.dll
NMOCK2_TEST_PDB=$(BUILDDIR)/NMock2.Test.pdb
NMOCK2_TEST_SRC:=$(shell find src/NMock2.Test -name '*.cs')
NMOCK2_TEST_RES=

NMOCK2_ACCEPTANCETESTS_DLL=$(BUILDDIR)/NMock2.AcceptanceTests.dll
NMOCK2_ACCEPTANCETESTS_PDB=$(BUILDDIR)/NMock2.AcceptanceTests.pdb
NMOCK2_ACCEPTANCETESTS_SRC:=$(shell find src/NMock2.AcceptanceTests -name '*.cs')
NMOCK2_ACCEPTANCETESTS_RES=


# common targets

all: NMock2 NMock2.Test NMock2.AcceptanceTests

test: unit-test acceptance-test

unit-test: $(NMOCK2_TEST_DLL) $(BUILDDIR)/nunit.framework.dll
	@echo Running unit tests
	@$(NUNIT) $(NMOCK2_TEST_DLL)

acceptance-test: $(NMOCK2_ACCEPTANCETESTS_DLL) $(BUILDDIR)/nunit.framework.dll
	@echo Running acceptance tests
	@$(NUNIT) $(NMOCK2_ACCEPTANCETESTS_DLL)

clean:
	rm -rf build/

again: clean all


# project names as targets

NMock2.Test: $(NMOCK2_TEST_DLL)
NMock2: $(NMOCK2_DLL)
NMock2.AcceptanceTests: $(NMOCK2_ACCEPTANCETESTS_DLL)

# don't allow commit if tests fail

commit: test
	cvs commit

# Build rules

$(NMOCK2_DLL): $(NMOCK2_SRC) $(NMOCK2_RES)
	mkdir -p $(BUILDDIR)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ $(NMOCK2_RES) $(NMOCK2_SRC)

$(NMOCK2_TEST_DLL): $(NMOCK2_DLL)
$(NMOCK2_ACCEPTANCETESTS_DLL): LIBS:=$(LIBS),$(NMOCK2_DLL)
$(NMOCK2_TEST_DLL): $(NMOCK2_TEST_SRC) $(NMOCK2_TEST_RES)
	$(MCS) $(MCSFLAGS) -r:$(NMOCK2_DLL) -target:library -out:$@ \
		$(NMOCK2_TEST_RES) $(NMOCK2_TEST_SRC)

$(NMOCK2_ACCEPTANCETESTS_DLL): $(NMOCK2_DLL)
$(NMOCK2_ACCEPTANCETESTS_DLL): $(BUILDDIR)/nunit.framework.dll
$(NMOCK2_ACCEPTANCETESTS_DLL): LIBS:=$(LIBS),$(NMOCK2_DLL)
$(NMOCK2_ACCEPTANCETESTS_DLL): $(NMOCK2_ACCEPTANCETESTS_SRC) $(NMOCK2_ACCEPTANCETESTS_RES)
	$(MCS) $(MCSFLAGS) -target:library -out:$@ \
		$(NMOCK2_ACCEPTANCETESTS_SRC) $(NMOCK2_ACCEPTANCETESTS_RES)


$(BUILDDIR)/%.dll: $(LIBDIR)/%.dll
	mkdir -p $(BUILDDIR)
	cp $< $@

.PHONY: all clean test unit-test acceptance-test NMock2 NMock2.Test NMock2.AcceptanceTests
