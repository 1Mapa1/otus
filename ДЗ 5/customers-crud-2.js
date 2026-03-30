import http from 'k6/http';
import { check, sleep, group } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://arch.homework';
const SEED_COUNT = Number(__ENV.SEED_COUNT || 300);

export const options = {
  scenarios: {
    seed_data: {
      executor: 'per-vu-iterations',
      vus: 1,
      iterations: SEED_COUNT,
      exec: 'seedData',
      maxDuration: '5m',
    },

    mixed_load: {
      executor: 'ramping-vus',
      startTime: '10s',
      exec: 'mixedFlow',
      stages: [
        { duration: '2m', target: 5 },
        { duration: '3m', target: 15 },
        { duration: '3m', target: 25 },
        { duration: '2m', target: 0 },
      ],
      gracefulRampDown: '30s',
    },
  },

  thresholds: {
    http_req_failed: ['rate<0.02'],
    http_req_duration: ['p(95)<500'],
    checks: ['rate>0.99'],

    'http_req_duration{name:GET /api/customers}': ['p(95)<400'],
    'http_req_duration{name:GET /api/customers/{id}}': ['p(95)<400'],
    'http_req_duration{name:POST /api/customers}': ['p(95)<500'],
    'http_req_duration{name:PUT /api/customers/{id}}': ['p(95)<500'],
    'http_req_duration{name:DELETE /api/customers/{id}}': ['p(95)<500'],
  },
};

function jsonParams(nameTag) {
  return {
    headers: { 'Content-Type': 'application/json' },
    tags: { name: nameTag },
  };
}

function requestParams(nameTag) {
  return {
    tags: { name: nameTag },
  };
}

function randomUser() {
  const suffix = `${__VU}-${__ITER}-${Math.floor(Math.random() * 1_000_000)}`;
  return {
    name: `user-${suffix}`,
    email: `user-${suffix}@test.local`,
  };
}

function customerIdForRead() {
  return 1 + ((__ITER + __VU) % Math.max(SEED_COUNT, 1));
}

export function seedData() {
  const user = randomUser();

  const res = http.post(
    `${BASE_URL}/api/customers`,
    JSON.stringify({
      name: user.name,
      email: user.email,
    }),
    jsonParams('POST /api/customers')
  );

  check(res, {
    'seed POST -> 201': (r) => r.status === 201,
  });
}

export function mixedFlow() {
  group('list customers', () => {
    const res = http.get(
      `${BASE_URL}/api/customers`,
      requestParams('GET /api/customers')
    );

    check(res, {
      'GET list -> 200': (r) => r.status === 200,
    });
  });

  sleep(1);

  group('read existing customer', () => {
    const id = customerIdForRead();

    const res = http.get(
      `${BASE_URL}/api/customers/${id}`,
      requestParams('GET /api/customers/{id}')
    );

    check(res, {
      'GET by id -> 200 or 404': (r) => r.status === 200 || r.status === 404,
    });
  });

  sleep(1);

  group('create-update-delete customer', () => {
    const user = randomUser();

    const createRes = http.post(
      `${BASE_URL}/api/customers`,
      JSON.stringify({
        name: user.name,
        email: user.email,
      }),
      jsonParams('POST /api/customers')
    );

    const createOk = check(createRes, {
      'POST -> 201': (r) => r.status === 201,
      'POST body json': (r) =>
        (r.headers['Content-Type'] || '').includes('application/json'),
    });

    if (!createOk) {
      return;
    }

    let body;
    try {
      body = createRes.json();
    } catch (e) {
      return;
    }

    const customerId = body.id;

    const getRes = http.get(
      `${BASE_URL}/api/customers/${customerId}`,
      requestParams('GET /api/customers/{id}')
    );

    check(getRes, {
      'GET created -> 200': (r) => r.status === 200,
    });

    const updateRes = http.put(
      `${BASE_URL}/api/customers/${customerId}`,
      JSON.stringify({
        name: `${user.name}-updated`,
        email: null,
      }),
      jsonParams('PUT /api/customers/{id}')
    );

    check(updateRes, {
      'PUT -> 204': (r) => r.status === 204,
    });

    const recheckRes = http.get(
      `${BASE_URL}/api/customers/${customerId}`,
      requestParams('GET /api/customers/{id}')
    );

    const recheckOk = check(recheckRes, {
      'GET recheck -> 200': (r) => r.status === 200,
    });

    if (recheckOk) {
      try {
        const recheckBody = recheckRes.json();

        check(recheckBody, {
          'updated name applied': (b) => b.name === `${user.name}-updated`,
          'updated email is null': (b) => b.email === null,
        });
      } catch (e) {
        // ignore parse failure, check already captured status
      }
    }

    const deleteRes = http.del(
      `${BASE_URL}/api/customers/${customerId}`,
      null,
      requestParams('DELETE /api/customers/{id}')
    );

    check(deleteRes, {
      'DELETE -> 204': (r) => r.status === 204,
    });
  });

  sleep(1);
}