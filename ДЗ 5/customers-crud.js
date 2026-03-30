import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  scenarios: {
    crud_flow: {
      executor: 'ramping-vus',
      stages: [
        { duration: '2m', target: 5 },
        { duration: '3m', target: 20 },
        { duration: '3m', target: 20 },
        { duration: '2m', target: 0 },
        ],
      gracefulRampDown: '30s',
    },
  },
  thresholds: {
    http_req_failed: ['rate<0.05'],      // ошибок < 5%
    http_req_duration: ['p(95)<800'],    // 95-й перцентиль < 800мс
    checks: ['rate>0.95'],               // успешных проверок > 95%
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://arch.homework';

function randomUser() {
  const id = `${__VU}-${__ITER}-${Date.now()}`;
  return {
    name: `pavel-${id}`,
    email: `pavel-${id}@test.local`,
  };
}

export default function () {
  const user = randomUser();

  // 1. CREATE
  const createPayload = JSON.stringify({
    name: user.name,
    email: user.email,
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  const createRes = http.post(`${BASE_URL}/api/customers`, createPayload, params);

  const createOk = check(createRes, {
    'POST /api/customers -> 201': (r) => r.status === 201,
    'POST response is json': (r) => r.headers['Content-Type'] && r.headers['Content-Type'].includes('application/json'),
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

  check(body, {
    'POST body has id': (b) => b.id !== undefined && b.id !== null,
    'POST body has name': (b) => b.name !== undefined,
    'POST body has email': (b) => b.email !== undefined,
    'POST body has dateOfBirth': (b) => Object.prototype.hasOwnProperty.call(b, 'dateOfBirth'),
  });

  // 2. GET
  const getRes = http.get(`${BASE_URL}/api/customers/${customerId}`);
  check(getRes, {
    'GET /api/customers/{id} -> 200': (r) => r.status === 200,
  });

  // 3. UPDATE
  const updatePayload = JSON.stringify({
    name: `${user.name}-updated`,
    email: null,
  });

  const updateRes = http.put(`${BASE_URL}/api/customers/${customerId}`, updatePayload, params);
  check(updateRes, {
    'PUT /api/customers/{id} -> 204': (r) => r.status === 204,
  });

  // 4. RECHECK
  const recheckRes = http.get(`${BASE_URL}/api/customers/${customerId}`);
  let recheckBody = null;

  const recheckOk = check(recheckRes, {
    'Recheck GET -> 200': (r) => r.status === 200,
  });

  if (recheckOk) {
    try {
      recheckBody = recheckRes.json();
    } catch (e) {
      recheckBody = null;
    }
  }

  if (recheckBody) {
    check(recheckBody, {
      'Name updated': (b) => b.name === `${user.name}-updated`,
      'Email is null after update': (b) => b.email === null,
    });
  }

  // 5. DELETE
  const deleteRes = http.del(`${BASE_URL}/api/customers/${customerId}`);
  check(deleteRes, {
    'DELETE /api/customers/{id} -> 204': (r) => r.status === 204,
  });

  sleep(1);
}