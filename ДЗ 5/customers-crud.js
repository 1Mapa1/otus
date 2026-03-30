import http from 'k6/http';
import { check, sleep, group } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://arch.homework';
const SEED_COUNT = Number(__ENV.SEED_COUNT || 10000); // пока 10 для диагностики
const LOAD_PROFILE = String(__ENV.LOAD_PROFILE || 'stress');

let postErrorLogCount = 0;
const POST_ERROR_LOG_LIMIT = 5;

function buildStages(profile) {
  switch (profile) {
	case 'stress':
		return [
		  { duration: '2m', target: 50 },
		  { duration: '2m', target: 100 },
		  { duration: '2m', target: 150 },
		  { duration: '2m', target: 200 },
		  { duration: '2m', target: 250 },
		  { duration: '6m', target: 250 }
		];
    case '100':
      return [
        { duration: '2m', target: 20 },
        { duration: '3m', target: 50 },
        { duration: '3m', target: 100 },
        { duration: '2m', target: 0 },
      ];
    case '50':
      return [
        { duration: '2m', target: 10 },
        { duration: '3m', target: 25 },
        { duration: '3m', target: 50 },
        { duration: '2m', target: 0 },
      ];
    case '30':
    default:
      return [
        { duration: '2m', target: 5 },
        { duration: '3m', target: 15 },
        { duration: '3m', target: 30 },
        { duration: '2m', target: 0 },
      ];
  }
}

export const options = {
  scenarios: {
    mixed_load: {
      executor: 'ramping-vus',
      exec: 'mixedFlow',
      stages: buildStages(LOAD_PROFILE),
      gracefulRampDown: '30s',
      startTime: '0s',
    },
  },

  thresholds: {
    http_req_failed: ['rate<0.02'],
    http_req_duration: ['p(95)<700'],
    checks: ['rate>0.99'],
  },

  setupTimeout: '10m',
  teardownTimeout: '10m',
};

function jsonParams(nameTag) {
  return {
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
    },
    tags: { name: nameTag },
  };
}

function requestParams(nameTag) {
  return {
    headers: {
      Accept: 'application/json',
    },
    tags: { name: nameTag },
  };
}

function randomInt(max) {
  return Math.floor(Math.random() * max);
}

function randomUser(prefix = 'user', index = 0) {
  const suffix = `${prefix}-${index}-${Date.now()}-${randomInt(1_000_000)}`;

  return {
    name: `name-${suffix}`,
    email: `${suffix}@test.local`
  };
}

function safeJson(res) {
  try {
    return res.json();
  } catch (_) {
    return null;
  }
}

function extractIdFromLocation(locationHeader) {
  if (!locationHeader) return null;

  const match = String(locationHeader).match(
    /\/api\/customers\/([0-9a-fA-F-]{36})(?:\/)?$/
  );

  return match ? match[1] : null;
}

function extractCustomerId(res) {
  const body = safeJson(res);

  if (body && body.id) {
    return String(body.id);
  }

  const location =
    res.headers.Location ||
    res.headers.location ||
    res.headers['Location'] ||
    res.headers['location'];

  return extractIdFromLocation(location);
}

function logPostFailureOnce(res, payload) {
  if (postErrorLogCount >= POST_ERROR_LOG_LIMIT) return;
  postErrorLogCount++;

  console.error(
    [
      'POST /api/customers failed',
      `status=${res.status}`,
      `body=${res.body}`,
      `payload=${JSON.stringify(payload)}`,
    ].join(' | ')
  );
}

function createCustomer(prefix = 'user', index = 0) {
  const user = randomUser(prefix, index);

  const payload = {
    name: user.name,
    email: user.email
  };

  const res = http.post(
    `${BASE_URL}/api/customers`,
    JSON.stringify(payload),
    jsonParams('POST /api/customers')
  );

  const ok = check(res, {
    'POST -> 201': (r) => r.status === 201,
  });

  if (!ok) {
    logPostFailureOnce(res, payload);
    return { ok: false, res, id: null, user };
  }

  const id = extractCustomerId(res);

  if (!id) {
    console.error(
      `POST returned 201 but id was not found | body=${res.body} | location=${res.headers.Location || res.headers.location || ''}`
    );
    return { ok: false, res, id: null, user };
  }

  return { ok: true, res, id, user };
}

function pickSeedId(seedIds) {
  if (!seedIds || seedIds.length === 0) {
    return null;
  }

  return seedIds[randomInt(seedIds.length)];
}

export function setup() {
  const seedIds = [];

  console.log(`Setup started. BASE_URL=${BASE_URL}, SEED_COUNT=${SEED_COUNT}`);

  for (let i = 0; i < SEED_COUNT; i++) {
    const created = createCustomer('seed', i);

    if (created.ok && created.id) {
      seedIds.push(created.id);
    }
  }

  console.log(`Setup finished. Created seedIds=${seedIds.length}`);

  if (seedIds.length === 0) {
    throw new Error('Setup failed: no seed customers created');
  }

  return { seedIds };
}

export function mixedFlow(data) {
  const seedIds = data.seedIds || [];

  group('read existing seeded customer', () => {
    const id = pickSeedId(seedIds);

    if (!id) return;

    const res = http.get(
      `${BASE_URL}/api/customers/${id}`,
      requestParams('GET /api/customers/{id}')
    );

    check(res, {
      'GET seeded by guid -> 200': (r) => r.status === 200,
    });
  });

  sleep(1);

  group('create-update-delete customer', () => {
    const created = createCustomer('flow', __ITER);

    if (!created.ok || !created.id) {
      return;
    }

    const customerId = created.id;
    const user = created.user;

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
        email: null
      }),
      jsonParams('PUT /api/customers/{id}')
    );

    check(updateRes, {
      'PUT -> 204': (r) => r.status === 204,
    });

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

export function teardown(data) {
  const seedIds = (data && data.seedIds) || [];

  for (const id of seedIds) {
    http.del(
      `${BASE_URL}/api/customers/${id}`,
      null,
      requestParams('DELETE /api/customers/{id}')
    );
  }

  console.log(`Teardown finished. Deleted seedIds=${seedIds.length}`);
}