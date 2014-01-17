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

#### Thread and Task Parallel

Thread and Task Parallel are both supported. Whichever one you want to use,
feel more comfortable with, or is more appropriate to your situation, is
completely up to you. We will break that down in a moment.

### IServiceWorker

The IServiceWorker interface exposes a Task and a handful of service hooks,
and represents a context of sorts for a single body-of-work that you want done
through the Extensions.

### AdaptableServiceWorker

AdaptableServiceWorker fleshes out the IServiceWorker a bit. These Extensions
do depend upon the Microsoft [Task Parallel Library](http://msdn.microsoft.com/en-us/library/dd460717.aspx) (TPL).

In simple terms, AdaptableServiceWorker fleshes out the bridge between
ServiceBase API and the ServiceWorkers themselves. It does so in a manner
agnostic of either Thread or Task Parallel.

#### MayContinue

We also use a ManualResetEvent, exposed through a **bool MayContinue()**
method, in order to determine whether to continue processing.

### AdaptableTaskParallelServiceWorker

Task Parallel is adapted into the ServiceProcess.Extensions. As analogy
runs, tasks are good when you want to make a sandwich, type thing. For
simple, one-off, shorter-lived, request-fulfillment type thing.

As an aside, TPL is available in .NET 4.0 and beyond. If you are supporting a
.NET 3.5 environment, TPL was back-ported, which I've included as a Nuget
reference. However, much of my testing and usage is in .NET 4.0, and so the 3.5
stuff is only partially verified. Feel free to exercise it. If you need or want
to contribute, let me know, I'd be happy to let you. If you are not yet running
with at least .NET 3.5, well, you should catch up with the rest of the .NET
world.

#### CancelToken

We use a **CancellationTokenSource CancelToken { get; }** property in order to
communicate whether the worker should stop processing.

#### NewTask

Start by deciding what your service
[Task](http://msdn.microsoft.com/en-us/library/system.threading.tasks.task.aspx)
should do. A [TaskScheduler](http://msdn.microsoft.com/en-us/library/system.threading.tasks.taskscheduler.aspx)
is provided for convenience if you should happen to want to configure
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

### AdaptableThreadServiceWorker

Thread is adapted into the ServiceProcess.Extensions. Threads are analogous
to hiring a chef to run your kitchen, type thing.

Thread workers need to be able to signal, or report, when they have completed.
Internally this is handled by the
[EventWaitHandle](http://msdn.microsoft.com/en-us/library/system.threading.eventwaithandle.aspx)
mechanism in the form of [ManualResetEvents](http://msdn.microsoft.com/en-us/library/system.threading.manualresetevent.aspx).
Externally, we implement **bool HasCompleted()**. Code is in place which waits
for the workers to all be completed.

Then coordinate with ServiceBasethrough through a combination of
**IsStopRequested** and **SetCompleted** calls from your thread code.
Check whether stop has been requested at strategic times during your
service loop. You are free to call completed when you want, but I
usually do this in the **finally** clause of a **try/catch** block.

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

ProductionServiceRunner is also Thread and Task agnostic. These concerns are
pretty well handled by AdaptableServiceBase derived classes. Interactive runners,
on the other hand, do concern themselves with Thread and Task issues.

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

##### InteractiveThreadServiceRunner and InteractiveTaskParallelServiceRunner

Thread and Task Parallel interactive ServiceRunners handle their Thread and Task
concerns, respectively. Basically, they verify that the workers are all Thread or Task
oriented, and also wait for the workers to be complete in a Thread or Task specific
manner.

That's about all. The rest is pretty well boiler plate InteractiveServiceRunnerBase.

#### Deciding Which ServiceRunner To Use

Here is a tip: use [Environment.UserInteractive](http://msdn.microsoft.com/en-us/library/system.environment.userinteractive.aspx)
in order to determine which IServiceRunner to present to your application.
Environment.UserInteractive allows you to decide between the interactive or
production (i.e. non-interactive) IServiceRunner. It does not matter whether
this is wired through a Dependency-Injection container, or whether you have
a simple factory method, or even a property.

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

I have included a sample harness application demonstrating basic usage. This is
basically showing a do-nothing service with only the absolute necessary
plumbing involved. I have been successful running several harness workers.
I have not yet attempted to run a couple different services themselves, but
I don't have any reason to doubt these Extensions would not scale at the
moment. This would be an interesting area to demonstrate in subsequent
revisions.
