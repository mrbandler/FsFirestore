namespace FsFirestore

module Query =
    
    open Google.Cloud.Firestore

    /// Returns a query with a 'end at' condition.
    let endAt (fields: obj[]) (query: Query) =
        query.EndAt(fields)

    /// Returns a query with a 'end before' condition.
    let endBefore (fields: obj[]) (query: Query) =
        query.EndBefore(fields)

    /// Returns a query with a 'start at' condition.
    let startAt (fields: obj[]) (query: Query) =
        query.StartAt(fields)

    /// Returns a query with a 'start after' condition.
    let startAfter (fields: obj[]) (query: Query) =
        query.StartAfter(fields)

    /// Returns a query with a limit condition.
    let limit value (query: Query) =
        query.Limit(value)

    /// Returns a query with a offset condition.
    let offset value (query: Query) =
        query.Offset(value)

    /// Returns a query with a order (asc) condition.
    let orderBy (field: string) (query: Query) =
        query.OrderBy(field)
       
    /// Returns a query with a order (desc) condition.
    let orderByDescending (field: string) (query: Query) =
        query.OrderByDescending(field)

    /// Returns a query with a specific field selection condition.
    let select (fields: string[]) (query: Query) =
        query.Select(fields)

    /// Returns a query with a condition of 'where equal to'.
    let whereEqualTo (field: string) value (query: Query) =
        query.WhereEqualTo(field, value)

    /// Returns a query with a condition of 'where greater then'.
    let whereGreaterThen (field: string) value (query: Query) =
        query.WhereGreaterThan(field, value)
    
    /// Returns a query with a condition of 'where greater then or equal to'.
    let whereGreaterThenOrEqualTo (field: string) value (query: Query) =
        query.WhereGreaterThanOrEqualTo(field, value);

    /// Returns a query with a condition of 'where less then'.
    let whereLessThen (field: string) value (query: Query) =
        query.WhereLessThan(field, value)
    
    /// Returns a query with a condition of 'where less then or equal to'.
    let whereLessThenOrEqualTo (field: string) value (query: Query) =
        query.WhereLessThanOrEqualTo(field, value)

    