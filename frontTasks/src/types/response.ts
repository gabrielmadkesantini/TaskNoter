export type Response<T> = {
    data: T;
    status: number;
    statusText: string;
    headers: Headers;
    request: XMLHttpRequest;
}