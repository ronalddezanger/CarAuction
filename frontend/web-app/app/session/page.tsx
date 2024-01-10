import React from 'react'
import Heading from '../components/Heading';
import { getSession, getTokenWorkaround } from '../actions/authActions';
import AuthTest from './AuthTest';

export default async function Session() {
    const session = await getSession(); 
    const token = await getTokenWorkaround();
  return (
    <div>
        <Heading title='Session dashboard' />
        <div className='bg-blue-200 border-2 border-blue-500'>
            <h3 className='text-lg p-2'>Session data</h3>
            <pre className='p-4'>{JSON.stringify(session, null, 2)}</pre>
        </div>
        <div className='mt-4'>
          <AuthTest />
        </div>
        <div className='mt-4 bg-green-200 border-2 border-green-500'>
            <h3 className='text-lg p-2'>Token data</h3>
            <pre className='overflow-auto p-4'>{JSON.stringify(token, null, 2)}</pre>
        </div>
    </div>
  )
}
