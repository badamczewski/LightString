# Light String 

NOTE: This project as of now is in hevy developement and cannot be even considered alpha.

This is a library that aims to bring a fast and low allocation (very little gen0 allocations) strings to .NET by reusing the current string structure where it can and replacing it with more specialized structure where there is absolutly no way to use standard strings.

Although Light String is based mostly on system string (and a few other custom string types) it changes the way that strings work so you need to know what are you doing and renember that from now on every string based action can have consequencess.

The details will be provided in documentation wiki later.
