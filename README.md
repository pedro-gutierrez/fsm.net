# fsm.net

A finite state machine DSL written in c#

```
In( "LOGGED_OUT" )
  .When( "login" ).Then( "LOGGED_IN" ).And( (args, data) => return args.Username );
 
In( "LOGGED_IN" )
  .When( "logout" ).Then( "LOGGED_OUT" ).And( (args, data) => return null );
```
