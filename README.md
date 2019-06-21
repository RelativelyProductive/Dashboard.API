This API largely serves as a 'middle man' between the Toggl API and our [Productivity Dashboard](https://github.com/RelativelyProductive/Dashboard).

[![Build status](https://ci.appveyor.com/api/projects/status/fsiuckh8fx2b2occ?svg=true)](https://ci.appveyor.com/project/jacobpretorius/dashboard-api)

## Bugs and Issues
Please see the [Github issues](https://github.com/RelativelyProductive/Dashboard.API/issues) to see if we are aware of it already, if not please [create a new issue](https://github.com/RelativelyProductive/Dashboard.API/issues/new).

## Contributing

For details on working with this repo see the [MANIFESTO](/Manifesto.md) first. Then feel free to see if an Issue is on the [Ongoing Development Roadmap](https://github.com/RelativelyProductive/Dashboard.API/projects/1) and mark it as in progress.

Make sure to [join our slack](https://join.slack.com/t/relativelyproductive/shared_invite/enQtNjcyODc1Nzc2Mjk1LTkwZjhjODMxZTU1MjBlY2JlYzBjMzI3NGJkOWI2MTc5N2ZhNzM1OGI0NjkyNGJjM2YyNWI5MDNmNjQ1ODZkMTE) and call out what you want to work on (or post nice ideas) on the #dashboard channel.

Also see the [Contributors](/Contributors.md) list.

### branching
Master is the current development head. Release or Release-Candidate branches will be made for deployment.

## Resources

You will be needing the [Toggl API Docs](https://github.com/toggl/toggl_api_docs) quite a bit.

## Tech Stack

Dotnet core 2.1 (to be upgraded to 3.0 on full release).

MSSQL Server (We don't need anything more advanced right now).