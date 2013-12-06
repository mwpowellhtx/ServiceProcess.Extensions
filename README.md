# ServiceProcess.Extensions

It's no fault of Microsoft when it provides frameworks that almost-permit
end-users (that's you and I) to bring up applications, services, etc. However,
not unlike other Microsoft-frameworks, those supporting Windows Services
developed using C# .NET fall into that category of "almost working". That is,
there's the service-event-framework exposed through a
[ServiceBase](http://msdn.microsoft.com/en-us/library/system.serviceprocess.servicebase.aspx),
[ServiceInstaller](http://msdn.microsoft.com/en-us/library/system.serviceprocess.serviceinstaller.aspx),
and so on, but that's about it.

Don't get me wrong, that's an excellent starting point, and I don't know if I
would go as far as to assert whether the out-of-the-box framework lends itself
to anti-pattern thinking. However, in practice, service application development
rarely, in my experience, ends with that. You'll inevitably want to invoke
additional hooks in order to really wire your application together well, in a
manner in which it can be more easily maintained. Hence my motivation for
Extensions.

## Architectural Design

Designing the Extensions, I considered how best to follow a [S.O.L.I.D.]
(http://en.wikipedia.org/wiki/SOLID_%28object-oriented_design%29) approach.

### Caveats

When developing Windows Service applications, it is common to want to be able
to do a couple of things:

* Self-install the Windows Service, usually through a command-line argument.

  * Including install and uninstall options.

* Run the service in a debug (i.e. interactive) mode, apart from the
[ServiceController](http://msdn.microsoft.com/en-us/library/system.serviceprocess.servicecontroller.aspx).

Pulling it all together, I wanted to provide sufficient framework elements
while stopping just short of how you actually want to process your
command-line-arguments; as well as, stopping just short of wiring up your
service application.

What do I mean by that? I'm glad you asked:

#### Command Line Arguments

* There is sufficient framework to receive your command-line arguments,
stopping just short of actually processing them. What you do with them
is up to you.

  * I like to use [NDesk.Options](http://www.ndesk.org/Options), for
  library, per se. Yes, at an application level; no, at an Extensions level.
  instance, but this should not be a dependency for a Service-Extensions

#### Run Hooks

* There is sufficient framework to determine whether to install, uninstall,
debug, etc, your service application. However, how that is determined is
completely up to you.

    * Case in point: the **bool RunInstaller { get; }** property captures
	whether an installer could run, but I do not tell you that the
	command-line option should be named "/install", or "/i", or "-i", or
	whatever. Or whatever other exotic way of determining an argument.

#### Application Plumbing

* Similarly, if your application has any kind of complexity associated with it,
I usually, not always but usually, support the notion of using a good
[Dependency Injection](http://en.wikipedia.org/wiki/Dependency_injection)
framework.

  * I've used [Autofac](http://autofac.org/) and I also happen to like
  [Ninject](http://www.ninject.org/) (at the moment). There are others of
  comparable value. However, along similar lines, an Extensions framework
  should not dictate which container to use, much less whether to use one at
  all.

The sample code exercising the Extensions, for instance, references neither
options nor Dependency Injection dependencies. I'm not sure I would organize
my production code quite like that, however, without further decoupling the
services themselves from the console application harness. But that's a topic
for your discretion beyond the scope of this repository.

## Breaking It Down

Let's have a look at the Extensions framework elements.

### IServiceBase

The core interface exposes ServiceBase as an interface. Basically it's a
direct extraction of the [ServiceBase](http://msdn.microsoft.com/en-us/library/system.serviceprocess.servicebase.aspx)
class, but in such a way that permits designing to an interface.

### AdaptableServiceBase

By extension, AdaptableServiceBase is an abstract base class. It extends
ServiceBase and implements IServiceBase.

Anything ServiceBase, whether AdaptableServiceBase or not, is ServiceController
scaffolding. From a single-responsibility perspective, that's about all it
should ever do.

The constructor receives an IServiceWorker enumeration; we will cover
IServiceWorker in a moment. As an enumeration, it is possible for a single
ServiceBase to host multiple workers; no assumptions whatsoever are made
regarding whether, much less how, those workers should interact with each
other.

After careful consideration, I also decided to seal the OnXYZ-style event
notification, while at the same time exposing a corresponding set of event
handlers. In other words, the class is closed for modification, while opening
the application up for extensibility.

### IServiceWorker

The IServiceWorker interface exposes a Task and a handful of service hooks,
and represents a context of sorts for a single body-of-work that you want done
through the Extensions.

### AdaptableServiceWorker

AdaptableServiceWorker fleshes out the IServiceWorker a bit. These Extensions
do depend upon the Microsoft [Task Parallel Library](http://msdn.microsoft.com/en-us/library/dd460717.aspx) (TPL).

As an aside, TPL is available in .NET 4.0 and beyond. If you are supporting a
.NET 3.5 environment, TPL was back-ported, which I've included as a Nuget
reference. However, much of my testing and usage is in .NET 4.0, and so the 3.5
stuff is only partially verified. Feel free to exercise it. If you need or want
to contribute, let me know, I'd be happy to let you.

Most services, although long-running, are also iterative in some way, shape, or
form. If yours is not, you should consider how to do so, in order for the
service to remain responsive to ServiceController events. Responsiveness is
achieved using the following protected interfaces.

#### CancelToken

We use a CancellationTokenSource property named CancelToken in order to
communicate whether the worker should stop processing.

#### MayContinue

We also use a ManualResetEvent, exposed through a **bool MayContinue()**
method, in order to determine whether to continue processing.

#### NewTask

Start by deciding what your service Task should do. A TaskSchedule is also
provided for convenience if you should happen to want to configure
scheduling at all.

The following is a self-explanatory, though admittedly over-simplified,
example. Check the token at least once while not-may-continue (i.e. paused).
Then do something, or in this case, efficiently wait. Return from the action
when cancel is requested.

```C#
protected override Task NewTask(TaskScheduler scheduler)
{
    Action start = () =>
    {
       while (true)
       {
           do
           {
               if (CancelToken.IsCancellationRequested) return;
           } while (!MayContinue(TimeSpan.FromMilliseconds(100)));

           Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }
    };

    return new Task(start, CancelToken.Token);
}
```

If your Task needs to work with parameters or anything of this nature, TPL
allows that since Task is the base class.

### IServiceRunner

Remember how I explained that the Extensions framework ought not dictate how to
wire up your application? Okay, that was partially true. In practice, I have
found that the framework works best when you prefer IServiceRunner first,
followed by command-line-arguments parsing subordinate to that. For some
subscribers, that may require turning your code inside out. Allow me to
encourage you: DO IT! You will not regret it. Okay, enough about that.

Basically, IServiceRunner exposes two methods: TryParse and Run. That's about
all it needs to do. In addition, distinction between interactive and production
(i.e. non-interactive) must also be made. All the building blocks are present
and accounted for that allow you to wire up concrete or dependency-injected
instances as you see fit.

#### ProductionServiceRunner

ProductionServiceRunner is pretty well self-contained. You may extend it
further if you so desire, i.e. you want to process arguments, or things of
this nature. However, it pretty well contains the code that receives the
IServiceBase instances and runs them.

#### InteractiveServiceRunnerBase

InteractiveServiceRunnerBase is mostly self-contained, but does nothing to
process command-line arguments. Furthermore, it is abstract, so you should
extend this at a minimum. You may process arguments if you so desire, such
as to install, uninstall, etc.

InteractiveServiceRunnerBase exposes basic interactive functionality when
running in debug mode (i.e. non-install and non-uninstall), allowing you to
**P**ause the workers, **R**esume the workers, or **Q**uit the service
altogether.

You are expected to implement the RunInstaller and RunUninstaller properties,
but the framework does not dictate how they should be implemented.

Finally, TryParse comes first, but again, you are free to decide whether, much
less how, to process the command-line arguments.

#### Deciding Which ServiceRunner To Use

Here is a tip: use [Environment.UserInteractive](http://msdn.microsoft.com/en-us/library/system.environment.userinteractive.aspx)
in order to determine which IServiceRunner to present to your application. It
does not matter whether this is wired through a Dependency-Injection container,
or whether you have a simple factory method, or even a property.
Environment.UserInteractive allows you to decide between the interactive or
production (i.e. non-interactive) IServiceRunner.

After which point, the actual body of your service application Main method is
pretty straightforward:

```C#
private static IServiceRunner GetServiceRunner()
{
    if (Environment.UserInteractive)
        return new InteractiveServiceRunner();
	return new ProductionServiceRunner();
}

public static void Main(string[] args)
{
    using (var runner = GetServiceRunner())
    {
        if (!runner.TryParse(args)) return;
        runner.Run();
    }
}
```

## Recommended Architecture

Of course it is completely up to you, although you may technically be able to
host multiple services in the same service assembly, if your project is at all
complicated, and they sometimes are, I suggest separating each service
including installer into a separate assembly, then referencing that from your
harness. Further decoupling, such as through a plugin-style adapter, is likely
overkill, in my opinion, especially for a service application, but again, that
is your adventure.

### Sample Code

I have included a sample application demonstrating basic usage. This is
basically showing a do-nothing service with the absolutely necessary plumbing
involved. I have been successful running several harness workers. I have not
yet attempted to run a couple different services themselves, but I don't have
any reason to doubt these Extensions would not scale at the moment. This would
be an interesting area to demonstrate in subsequent revisions.

## Disclaimer

Definition: The term "person" as used in this section includes not only a natural person but any entity, (including natural persons), who holds a copyright in, or published, developed, designed, modified, distributed, redistributed, or in any way contributed to this software or program.

NO WARRANTIES: TO THE EXTENT PERMITTED BY APPLICABLE LAW, NEITHER WSDOT, NOR ANY PERSON, EITHER EXPRESSLY OR IMPLICITLY, WARRANTS ANY ASPECT OF THIS SOFTWARE OR PROGRAM, INCLUDING ANY OUTPUT OR RESULTS OF THIS SOFTWARE OR PROGRAM. UNLESS AGREED TO IN WRITING. THIS SOFTWARE AND PROGRAM IS BEING PROVIDED "AS IS", WITHOUT ANY WARRANTY OF ANY TYPE OR NATURE, EITHER EXPRESS OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE, AND ANY WARRANTY THAT THIS SOFTWARE OR PROGRAM IS FREE FROM DEFECTS.

ASSUMPTION OF RISK: THE RISK OF ANY AND ALL LOSS, DAMAGE, OR UNSATISFACTORY PERFORMANCE OF THIS SOFTWARE OR PROGRAM RESTS WITH YOU AS THE USER. TO THE EXTENT PERMITTED BY LAW, NEITHER WSDOT, NOR ANY PERSON EITHER EXPRESSLY OR IMPLICITLY, MAKES ANY REPRESENTATION OR WARRANTY REGARDING THE APPROPRIATENESS OF THE USE, OUTPUT, OR RESULTS OF THE USE OF THIS SOFTWARE OR PROGRAM IN TERMS OF ITS CORRECTNESS, ACCURACY, RELIABILITY, BEING CURRENT OR OTHERWISE. NOR DO THEY HAVE ANY OBLIGATION TO CORRECT ERRORS, MAKE CHANGES, SUPPORT THIS SOFTWARE OR PROGRAM, DISTRIBUTE UPDATES, OR PROVIDE NOTIFICATION OF ANY ERROR OR DEFECT, KNOWN OR UNKNOWN. IF YOU RELY UPON THIS SOFTWARE OR PROGRAM, YOU DO SO AT YOUR OWN RISK, AND YOU ASSUME THE RESPONSIBILITY FOR THE RESULTS. SHOULD THIS SOFTWARE OR PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL LOSSES, INCLUDING, BUT NOT LIMITED TO, ANY NECESSARY SERVICING, REPAIR OR CORRECTION OF ANY PROPERTY INVOLVED.

DISCLAIMER: IN NO EVENT, UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SHALL WSDOT, OR ANY PERSON BE LIABLE FOR ANY LOSS, EXPENSE OR DAMAGE, OF ANY TYPE OR NATURE ARISING OUT OF THE USE OF, OR INABILITY TO USE THIS SOFTWARE OR PROGRAM, INCLUDING, BUT NOT LIMITED TO, CLAIMS, SUITS OR CAUSES OF ACTION INVOLVING ALLEGED INFRINGEMENT OF COPYRIGHTS, PATENTS, TRADEMARKS, TRADE SECRETS, OR UNFAIR COMPETITION.

INDEMNIFICATION: TO THE EXTEND PERMITTED BY LAW THROUGH THIS LICENSE, YOU, THE LICENSEE, AGREE TO INDEMNIFY AND HOLD HARMLESS WSDOT, ITS OFFICIALS AND EMPLOYEES, AND ANY PERSON FROM AND AGAINST ALL CLAIMS, LIABILITIES, LOSSES, CAUSES OF ACTION, DAMAGES, JUDGMENTS, AND EXPENSES, INCLUDING THE REASONABLE COST OF ATTORNEYS’ FEES AND COURT COSTS, FOR INJURIES OR DAMAGES TO THE PERSON OR PROPERTY OF THIRD PARTIES, INCLUDING, WITHOUT LIMITATIONS, CONSEQUENTIAL DAMAGES AND ECONOMIC LOSSES, THAT ARISE OUT OF OR IN CONNECTION WITH YOUR USE, MODIFICATION, OR DISTRIBUTION OF THIS SOFTWARE OR PROGRAM, ITS OUTPUT, OR ANY ACCOMPANYING DOCUMENTATION. 
