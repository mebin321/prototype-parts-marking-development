import { Mutex, appendPaginationQueryParameters, extractErrorDetails } from '../utilities';

const sleep = async (milliseconds: number) => await new Promise(resolve => setTimeout(resolve, milliseconds));

const createError = (message: string, properties: object = {}) =>
{
  return Object.assign(new Error(message), properties);
};

describe('Mock', () =>
{
  it('should not block if not locked', async () =>
  {
    const mutex = new Mutex();
    const unlock = await mutex.lock();
    unlock();
    unlock();
  });

  it('should block concurrent calls with Mutex.lock()', async () =>
  {
    const mutex = new Mutex();

    const generateSynchronizedTestFunction = (timeout: number, start: () => void, end: () => void) => async () =>
    {
      start();
      const unlock = await mutex.lock();
      await sleep(timeout);
      unlock();
      end();
    };

    const fooStart = jest.fn();
    const fooEnd = jest.fn();
    const fooTimeout = 1000;
    const foo = generateSynchronizedTestFunction(fooTimeout, fooStart, fooEnd);
    const barStart = jest.fn();
    const barEnd = jest.fn();
    const barTimeout = 500;
    const bar = generateSynchronizedTestFunction(barTimeout, barStart, barEnd);
    const bazStart = jest.fn();
    const bazEnd = jest.fn();
    const bazTimeout = 100;
    const baz = generateSynchronizedTestFunction(bazTimeout, bazStart, bazEnd);

    const startTime = Date.now();
    const fooPromise = foo();
    const barPromise = bar();
    const bazPromise = baz();
    await Promise.all([fooPromise, barPromise, bazPromise]).then(() =>
    {
      // check total execution time
      const endTime = Date.now();
      const duration = endTime - startTime;
      expect(duration).toBeGreaterThanOrEqual(fooTimeout + barTimeout + bazTimeout);

      // ensure functions were started in correct order
      expect(fooStart).toHaveBeenCalledBefore(barStart);
      expect(barStart).toHaveBeenCalledBefore(bazStart);

      // ensure execution was started in parallel
      expect(barStart).toHaveBeenCalledBefore(fooEnd);
      expect(bazStart).toHaveBeenCalledBefore(barEnd);

      // ensure functions were finished in correct order
      expect(fooEnd).toHaveBeenCalledBefore(barEnd);
      expect(barEnd).toHaveBeenCalledBefore(bazEnd);
    });
  });

  it('should block concurrent calls with Mutex.dispatch()', async () =>
  {
    const mutex = new Mutex();

    const generateSynchronizedTestFunction = (timeout: number, start: () => void, end: () => void) => async () =>
    {
      start();
      await mutex.dispatch(async () =>
      {
        await sleep(timeout);
      });
      end();
    };

    const fooStart = jest.fn();
    const fooEnd = jest.fn();
    const fooTimeout = 1000;
    const foo = generateSynchronizedTestFunction(fooTimeout, fooStart, fooEnd);
    const barStart = jest.fn();
    const barEnd = jest.fn();
    const barTimeout = 500;
    const bar = generateSynchronizedTestFunction(barTimeout, barStart, barEnd);
    const bazStart = jest.fn();
    const bazEnd = jest.fn();
    const bazTimeout = 100;
    const baz = generateSynchronizedTestFunction(bazTimeout, bazStart, bazEnd);

    const startTime = Date.now();
    const fooPromise = foo();
    const barPromise = bar();
    const bazPromise = baz();
    await Promise.all([fooPromise, barPromise, bazPromise]).then(() =>
    {
      // check total execution time
      const endTime = Date.now();
      const duration = endTime - startTime;
      expect(duration).toBeGreaterThanOrEqual(fooTimeout + barTimeout + bazTimeout);

      // ensure functions were started in correct order
      expect(fooStart).toHaveBeenCalledBefore(barStart);
      expect(barStart).toHaveBeenCalledBefore(bazStart);

      // ensure execution was started in parallel
      expect(barStart).toHaveBeenCalledBefore(fooEnd);
      expect(bazStart).toHaveBeenCalledBefore(barEnd);

      // ensure functions were finished in correct order
      expect(fooEnd).toHaveBeenCalledBefore(barEnd);
      expect(barEnd).toHaveBeenCalledBefore(bazEnd);
    });
  });
});

describe('extractErrorDetails', () =>
{
  describe('additional information not available', () =>
  {
    it('should return empty string', () =>
    {
      const actualMessage = extractErrorDetails(undefined);
      expect(actualMessage).toBe('');
    });

    it('should return default error message ' +
    'on error w/o response', () =>
    {
      const expectedMessage = 'generic error message';
      const err = createError(expectedMessage);
      const actualMessage = extractErrorDetails(err);
      expect(actualMessage).toBe(expectedMessage);
    });

    it('should return default error message ' +
    'on error w/o response.data', () =>
    {
      const expectedMessage = 'generic error message';
      const err = createError(expectedMessage, { response: {} });
      const actualMessage = extractErrorDetails(err);
      expect(actualMessage).toBe(expectedMessage);
    });

    it('should return default error message ' +
    'on error w/o response.data.detail', () =>
    {
      const expectedMessage = 'generic error message';
      const err = createError(expectedMessage, { response: { data: {} } });
      const actualMessage = extractErrorDetails(err);
      expect(actualMessage).toBe(expectedMessage);
    });

    it('should return default error message ' +
    'on error w/o response.data.detail only w/ response.data.traceId', () =>
    {
      const expectedMessage = 'generic error message';
      const expectedTraceId = `trace-id-${Date.now()}`;
      const err = createError(expectedMessage, { response: { data: { traceId: expectedTraceId } } });
      const actualMessage = extractErrorDetails(err, true);
      expect(actualMessage).toBe(expectedMessage);
      expect(actualMessage).not.toContain(expectedTraceId);
    });
  });

  describe('additional information available', () =>
  {
    it('should return extended error message ' +
    'on error w/ response.data.detail but w/o response.data.traceId', () =>
    {
      const expectedMessage = 'extended error message';
      const err = createError('generic error message', { response: { data: { detail: expectedMessage } } });
      const actualMessage = extractErrorDetails(err, true);
      expect(actualMessage).toBe(expectedMessage);
    });

    it('should return extended error message w/ trace id ' +
    'on error w/ response.data.detail and w/ response.data.traceId', () =>
    {
      const expectedMessage = 'extended error message';
      const expectedTraceId = `trace-id-${Date.now()}`;
      const err = createError(
        'generic error message',
        { response: { data: { detail: expectedMessage, traceId: expectedTraceId } } });
      const actualMessage: string = extractErrorDetails(err, true);
      expect(actualMessage).toStartWith(expectedMessage);
      expect(actualMessage).toContain(expectedTraceId);
    });

    it('should not return trace id ' +
    'on error w/ response.data.traceId but includeTraceId=false', () =>
    {
      const expectedMessage = 'extended error message';
      const expectedTraceId = `trace-id-${Date.now()}`;
      const err = createError(
        'generic error message',
        { response: { data: { detail: expectedMessage, traceId: expectedTraceId } } });
      const actualMessage: string = extractErrorDetails(err, false);
      expect(actualMessage).toBe(expectedMessage);
      expect(actualMessage).not.toContain(expectedTraceId);
    });

    it('should return extended error message w/o title', () =>
    {
      const expectedMessage = 'extended error message';
      const err = createError(
        'generic error message',
        { response: { data: { detail: expectedMessage } } });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedMessage);
      expect(actualMessage).not.toContain(':');
    });

    it('should return title w/o generic error message', () =>
    {
      const expectedTitle = 'Something went wrong';
      const expectedMessage = 'generic error message';
      const err = createError(
        expectedMessage,
        { response: { data: { title: expectedTitle } } });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toBe(expectedTitle);
      expect(actualMessage).not.toContain(':');
    });

    it('should return title w/o error message', () =>
    {
      const expectedTitle = 'Something went wrong';
      const err = createError(
        '',
        { response: { data: { title: expectedTitle } } });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toBe(expectedTitle);
      expect(actualMessage).not.toContain(':');
    });

    it('should return status code in error title', () =>
    {
      const statusCode = 400;
      const err = createError(
        'generic error message',
        { response: { status: statusCode } });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toContain(statusCode.toString());
    });

    it('should return status code with status text in error title', () =>
    {
      const statusCode = 400;
      const statusText = 'Bad Request';
      const err = createError(
        'generic error message',
        { response: { status: statusCode, statusText: statusText } });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toContain(statusCode.toString());
      expect(actualMessage).toContain(statusText);
    });

    it('should not return status code w/o status text in error title', () =>
    {
      const statusText = 'Bad Request';
      const err = createError(
        'generic error message',
        { response: { statusText: statusText } });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).not.toContain(statusText);
    });

    it('should return errors text w/o title', () =>
    {
      const prototypesError = '"Prototypes" must not be empty.';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              errors: { Prototypes: [prototypesError] },
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toContain(prototypesError);
    });

    it('should return 0 errors in error text', () =>
    {
      const expectedTitle = 'One or more validation errors occurred';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              title: expectedTitle + '.',
              errors: {},
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedTitle);
      expect(actualMessage).toContain(':');
    });

    it('should return 1 error in error text', () =>
    {
      const expectedTitle = 'One or more validation errors occurred';
      const prototypesError = '"Prototypes" must not be empty.';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              title: expectedTitle + '.',
              errors: { Prototypes: [prototypesError] },
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedTitle);
      expect(actualMessage).toContain(prototypesError);
    });

    it('should return 2 errors in error text', () =>
    {
      const expectedTitle = 'One or more validation errors occurred';
      const prototypesError = '"Prototypes" must not be empty.';
      const prototypeSetsError = '"Prototype Sets" must not be empty.';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              title: expectedTitle + '.',
              errors:
              {
                Prototypes: [prototypesError],
                PrototypeSets: [prototypeSetsError],
              },
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedTitle);
      expect(actualMessage).toContain(prototypesError);
      expect(actualMessage).toContain(prototypeSetsError);
    });

    it('should return 2 prototypes errors in error text', () =>
    {
      const expectedTitle = 'One or more validation errors occurred';
      const prototypesError1 = '"Prototypes" must not be empty.';
      const prototypesError2 = '"Prototypes" must be given.';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              title: expectedTitle + '.',
              errors: { Prototypes: [prototypesError1, prototypesError2] },
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedTitle);
      expect(actualMessage).toContain(prototypesError1);
      expect(actualMessage).toContain(prototypesError2);
    });

    it('should return prototypes key with no errors', () =>
    {
      const expectedTitle = 'One or more validation errors occurred';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              title: expectedTitle + '.',
              errors: { Prototypes: [] },
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedTitle);
      expect(actualMessage).toContain('Prototypes');
    });

    it('should return text representation of errors property value not array', () =>
    {
      const expectedTitle = 'One or more validation errors occurred';
      const prototypesError = '"Prototypes" must not be empty.';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              title: expectedTitle + '.',
              errors: { Prototypes: prototypesError },
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedTitle);
      expect(actualMessage).toContain(prototypesError);
    });

    it('should return items from errors array (instead of object)', () =>
    {
      const expectedTitle = 'One or more validation errors occurred';
      const prototypesError = '"Prototypes" must not be empty.';
      const prototypeSetsError = '"Prototype Sets" must not be empty.';
      const err = createError(
        'generic error message',
        {
          response:
          {
            data:
            {
              title: expectedTitle + '.',
              errors: [prototypesError, prototypeSetsError],
            },
          },
        });
      const actualMessage: string = extractErrorDetails(err);
      expect(actualMessage).toStartWith(expectedTitle);
      expect(actualMessage).toContain(prototypesError);
      expect(actualMessage).toContain(prototypeSetsError);
    });
  });
});

describe('appendPaginationQueryParameters', () =>
{
  describe('relative URL', () =>
  {
    it('should append default page', () =>
    {
      const initialUrl = 'test/something';
      const paginatedUrl = appendPaginationQueryParameters(initialUrl);
      expect(paginatedUrl).toBe(`${initialUrl}?Page=1`);
    });

    it('should append given page', () =>
    {
      const initialUrl = 'test/something';
      const page = 3;
      const paginatedUrl = appendPaginationQueryParameters(initialUrl, page);
      expect(paginatedUrl).toBe(`${initialUrl}?Page=${page}`);
    });

    it('should append given page and page size', () =>
    {
      const initialUrl = 'test/something';
      const page = 3;
      const pageSize = 50;
      const paginatedUrl = appendPaginationQueryParameters(initialUrl, page, pageSize);
      expect(paginatedUrl).toBe(`${initialUrl}?Page=${page}&PageSize=${pageSize}`);
    });
  });

  describe('absolute URL', () =>
  {
    it('should append default page', () =>
    {
      const initialUrl = 'http://www.test.com/something';
      const paginatedUrl = appendPaginationQueryParameters(initialUrl);
      expect(paginatedUrl).toBe(`${initialUrl}?Page=1`);
    });

    it('should append given page', () =>
    {
      const initialUrl = 'http://www.test.com/something';
      const page = 3;
      const paginatedUrl = appendPaginationQueryParameters(initialUrl, page);
      expect(paginatedUrl).toBe(`${initialUrl}?Page=${page}`);
    });

    it('should append given page and page size', () =>
    {
      const initialUrl = 'http://www.test.com/something';
      const page = 3;
      const pageSize = 50;
      const paginatedUrl = appendPaginationQueryParameters(initialUrl, page, pageSize);
      expect(paginatedUrl).toBe(`${initialUrl}?Page=${page}&PageSize=${pageSize}`);
    });
  });
});
