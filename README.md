# Dapper.UnitOfWork
A simple unit of work implementation on top of Dapper, with some basic CQS in mind.

Starting with version 1.1.0.0 it also features a retry mechanism for sql transient exceptions, with a configurable exponential backoff.
