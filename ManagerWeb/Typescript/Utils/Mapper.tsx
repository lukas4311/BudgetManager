class Mapper {
    map<F, T>(from: F, to: T): T {
        for (const prop of Object.getOwnPropertyNames(from).filter(prop => prop !== "constructor"))
            to[prop] = from[prop];

        return to;
    }
}
