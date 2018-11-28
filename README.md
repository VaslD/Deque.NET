# Deque.NET (Forked from [DequeNET](https://github.com/dcastro/DequeNET))

## Introduction

Deque.NET provides a concurrent lock-free Deque C# implementation.

A Deque (**d**ouble-**e**nded **que**ue), is a data structure that allows insertion and removal of items on both ends. As such, `ConcurrentDeque<T>` supports 4 main operations (PushRight, PopRight, PushLeft and PopLeft) plus 2 getters (PeekRight and PeekLeft).

The library also offers a simpler `Deque<T>` (not thread safe), implemented as a ring buffer.
This implementation allows all 6 operations to be executed in constant time O(1).

## Revisions

Based on release 1.0.2 of the original project, this fork has made several changes to the original project:

1. Targets *.NET Standard 2.0* and uses *NUnit 3* for unit-testing.
2. Two collection classes are moved to *System.Collections* namespace. Both implementations seem good enough to be included in the standard library. (Honestly I did this so I can write less `using`s. )
3. `Deque<T>` and its abstract interface `IDeque<T>` no longer implements `ICollection<T>`. `ICollection<T>` defines an interface best suited for collections that provide unrestricted access (such as `List<T>`) to their elements, which isn't the case for a Deque. Instead, `Deque<T>` now implements only `ICollection` (the non-generic version) and `IReadOnlyCollection<T>`.
4. `ConcurrentDeque<T>` and its abstract interface `IConcurrentDeque<T>` no longer implements `IProducerConsumerCollection<T>`. `IProducerConsumerCollection<T>` defines a single entry and a single exit point for elements. This model works for `ConcurrentStack<T>` and `ConcurrentQueue<T>`, but doesn't hold true for `ConcurrentDeque<T>`. Instead, `ConcurrentDeque<T>` now implements only `ICollection` (the non-generic version) and `IReadOnlyCollection<T>`.

## The Algorithm

The implementation is based on the algorithm proposed by Maged M. Michael [1].
The algorithm uses the atomic primitive CAS (compare-and-swap) to achieve lock-freedom.
Because of this property, the algorithm guarantees system-wide progress (i.e., an operation will always complete within a finite number of steps) and is immune to deadlocks, unlike traditional mutual exclusion techniques.

Without contention, all four main operations run in constant time O(1).
Under contention by P processes, the operations' total work is O(P).
PeekRight and PeekLeft run in constant time regardless of contention.


[1] Michael, Maged, 2003, CAS-Based Lock-Free Algorithm for Shared Deques, *Euro-Par 2003 Parallel Processing*, v. 2790, p. 651-660, http://www.research.ibm.com/people/m/michael/europar-2003.pdf (December 22, 2013).


## Installation

Since this is a tailored duplicate of DequeNET for personal use, this project is not published on NuGet and never will be. The original DequeNET package is [available there](https://www.nuget.org/packages/DequeNET/).
