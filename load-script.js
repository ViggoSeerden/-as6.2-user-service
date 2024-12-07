import http from 'k6/http';
import { check } from 'k6';

export const options = {
    thresholds: {
        http_req_failed: ['rate<0.01'], // http errors should be less than 1%
        http_req_duration: ['p(95)<200'], // 95% of requests should be below 200ms
    },
    stages: [
        { duration: '5m', target: 100 },
        { duration: '10m', target: 100 },
        { duration: '5m', target: 0 },
    ],
    cloud: {
        projectID: `${process.env.CLOUD_PROJECT.toString()}`,
        name: `${process.env.PROJECT_NAME.toString()} ${process.env.BRANCH.toString()}: Load Test`
    }
};

export default function () {
    const url = `http://localhost:8080/api/users/self/email?email=${process.env.TEST_EMAIL.toString()}`;
    const params = {
        headers: {
            'Authorization': `Bearer ${process.env.TOKEN.toString()}`,
            'Content-Type': 'application/json',
        },
    };
    const res = http.get(url, params);
    check(res, { 'status was 200': (r) => r.status == 200 });
}